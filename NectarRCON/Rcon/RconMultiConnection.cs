﻿using NectarRCON.Core.Helper;
using NectarRCON.Export.Interfaces;
using NectarRCON.Interfaces;
using NectarRCON.Models;
using NectarRCON.Rcon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Windows.Controls;
using NectarRCON.Dp;

namespace NectarRCON.Services
{
    /// <summary>
    /// Rcon多客户端连接服务
    /// </summary>
    internal class RconMultiConnection : IRconConnection, IDisposable
    {
        private readonly RconSettingsDp _settingsDp = DpFile.LoadSingleton<RconSettingsDp>();
        public event MessageEvent? OnMessage;
        public event RconEvent? OnClosed;
        public event RconEvent? OnConnected;
        public event RconEvent? OnConnecting;

        private readonly IServerPasswordService _serverPasswordService;
        private readonly ILanguageService _languageService;
        private readonly IRconConnectionInfoService _rconConnectionInfoService;
        private readonly IMessageBoxService _messageBoxService;

        private readonly Dictionary<ServerInformation,IRconAdapter> _connections = new();
        private bool _isConnecting = false;

        public RconMultiConnection(IServerPasswordService serverPasswordService, ILanguageService languageService, IRconConnectionInfoService rconConnectionInfoService, IMessageBoxService messageBoxService)
        {
            _serverPasswordService = serverPasswordService;
            _languageService = languageService;
            _rconConnectionInfoService = rconConnectionInfoService;
            _messageBoxService = messageBoxService;
        }

        public void Close()
        {
            foreach(var connection in _connections)
            {
                connection.Value.Dispose();
            }
            _connections.Clear();
        }

        public void Connect()
        {
            Close();
            _isConnecting = true;
            var servers = _rconConnectionInfoService.GetInformation();
            try
            {
                foreach (ServerInformation info in servers)
                {
                    OnConnecting?.Invoke(info);
                    // 准备开始连接,先解析这个地址有没有SRV记录
                    string address = DNSHelpers.SRVQuery(info.Address);
                    if (string.IsNullOrEmpty(address)) // 如果没有SRV记录,就按照原来的样子设置服务器
                        address = $"{info.Address.Replace("localhost", "127.0.0.1")}:{info.Port}";
                    ServerPassword? serverPassword = _serverPasswordService.Get(info); // 从设置中读取Rcon密码
                    string password = serverPassword?.Password ?? string.Empty;

                    // 创建对应的Rcon客户端实例
                    // 目前支支持了Minecraft,后期会支持更多(嘛..主要是懒)
                    IRconAdapter adapter = AdapterHelpers.CreateAdapterInstance(info.Adapter)
                        ?? throw new InvalidOperationException($"adapter not found: {info.Adapter}");

                    string host = address.Split(":")[0];
                    int port = int.Parse(address.Split(":")[1]);

                    try
                    {
                        adapter.Connect(host, port);
                        if (!adapter.Authenticate(password))
                            throw new AuthenticationException($"Server: \"{info.Name}\"\n");
                        OnConnected?.Invoke(info);
                    }
                    catch (AuthenticationException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        OnClosed?.Invoke(info);
                        _messageBoxService.Show(ex, $"Server: \"{info.Name}\"");
                    }

                    //设置编码
                    adapter.SetEncoding(_settingsDp.Encoding.GetEncoding());
                    _connections.Add(info, adapter);
                }
            }
            finally
            {
                _isConnecting = false;
            }

        }

        public bool IsConnected()
            => _connections.Where(e => e.Value.IsConnected).Any();

        public bool IsConnecting()
            => _isConnecting;

        public void Send(string command)
        {
            foreach(var value in _connections)
            {
                IRconAdapter connection = value.Value;
                ServerInformation info = value.Key;
                if (connection.IsConnected)
                {
                    try
                    {
                        string result = connection.Run(command);
                        OnMessage?.Invoke(info, result);
                    }catch (IOException)
                    {
                        connection.Dispose(); // 内部会调用Close
                        OnClosed?.Invoke(info);
                        OnMessage?.Invoke(info, _languageService.GetKey("service.rcon.offline"));
                    }
                }
                else
                {
                    OnMessage?.Invoke(info, _languageService.GetKey("service.rcon.offline"));
                }
            }
        }

        public void Dispose()
        {
            Close();
        }
    }
}
