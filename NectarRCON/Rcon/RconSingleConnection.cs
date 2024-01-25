using NectarRCON.Core.Helper;
using NectarRCON.Export.Interfaces;
using NectarRCON.Interfaces;
using NectarRCON.Models;
using NectarRCON.Rcon;
using System;
using System.IO;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Windows;
using NectarRCON.Dp;
using Serilog;

namespace NectarRCON.Services;
public class RconSingleConnection(
    IServerPasswordService serverPasswordService,
    ILanguageService languageService,
    IRconConnectionInfoService rconConnectionInfoService)
    : IRconConnection
{
    private readonly RconSettingsDp _settingsDp = DpFile.LoadSingleton<RconSettingsDp>();

    private IRconAdapter? _rconClient
        = null;

    private bool _connecting;

    public event MessageEvent? OnMessage;
    public event RconEvent? OnClosed;
    public event RconEvent? OnConnected;
    public event RconEvent? OnConnecting;

    private ServerInformation _serverInformation = new();
    public void Close()
    {
        lock (this)
        {
            if (IsConnected() && !IsConnecting())
            {
                _connecting = false;
                _rconClient?.Dispose();
                _rconClient = null;
                OnClosed?.Invoke(_serverInformation);
            }
        }
    }
    public void Connect()
    {
        ServerInformation info = rconConnectionInfoService.GetLastInformation() ?? throw new ArgumentNullException("Internal error, please try again");
        _connecting = true;
        OnConnecting?.Invoke(info);
        try
        {
            if (IsConnected() && _rconClient != null)
                Close();
            // 准备开始连接,先解析这个地址有没有SRV记录
            string address = DNSHelpers.SRVQuery(info.Address);
            if (string.IsNullOrEmpty(address)) // 如果没有SRV记录,就按照原来的样子设置服务器
                address = $"{info.Address.Replace("localhost", "127.0.0.1")}:{info.Port}";


            ServerPassword? serverPassword = serverPasswordService.Get(info);
            string password = serverPassword?.Password ?? string.Empty;

            // 创建对应的Rcon客户端实例
            // 目前支支持了Minecraft,后期会支持更多(嘛..主要是懒)
            _rconClient = AdapterHelpers.CreateAdapterInstance(info.Adapter)
                ?? throw new InvalidOperationException($"adapter not found: {info.Adapter}");
            _rconClient.SetEncoding(_settingsDp.Encoding.GetEncoding());
            var host = address.Split(":")[0];
            var port = int.Parse(address.Split(":")[1]);

            Log.Information("[RconSingleConnection] Connecting to {host}:{port}", host, port);
            
            _rconClient.Connect(host, port); // 连接

            if (!_rconClient.Authenticate(password))
                throw new AuthenticationException();

            OnConnected?.Invoke(info);

            _serverInformation = info;
        }
        catch (FormatException ex)
        {
            MessageBox.Show(languageService.GetKey("ui.text.format_exception")
                .Replace("%s", ex.Message), languageService.GetKey("text.error"), MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            _connecting = false;
        }
    }

    public bool IsConnected()
        => _rconClient?.IsConnected ?? false;
    public bool IsConnecting()
        => _connecting;
    public void Send(string command)
    {
        if (IsConnected() && _rconClient != null)
        {
            try
            {
                string result = _rconClient.Run(command);
                OnMessage?.Invoke(_serverInformation, result);
            }
            catch (Exception ex)
            {
                Close();
                if (ex is SocketException or IOException && _settingsDp.AutoReconnect)
                {
                    Log.Information("Rcon连接已经断开,开始自动重连...");
                    try
                    {
                        Connect();
                    }
                    catch (Exception e)
                    {
                        Log.Error("自动重连失败 {err}", e);
                    }

                    if (IsConnected())
                    {
                        try
                        {
                            Log.Information("尝试重发命令...");
                            string result = _rconClient.Run(command);
                            OnMessage?.Invoke(_serverInformation, result);
                            Log.Information("重发命令成功 -> {command}", command);
                            return;
                        }
                        catch(Exception e)
                        {
                            Log.Error("尝试重发命令失败 {err}", e);
                        }
                    }
                }
                MessageBox.Show($"{languageService.GetKey("text.error")}\n{ex.Message}", ex.GetType().FullName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        else
        {
            Close();
            MessageBox.Show($"{languageService.GetKey("text.server.not_connect.text")}", languageService.GetKey("text.error"), MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
