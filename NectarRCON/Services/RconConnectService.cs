using NectarRCON.Interfaces;
using NectarRCON.Models;
using CoreRCON;
using System.Threading.Tasks;
using System.Net;
using System;
using System.Windows;
using Wpf.Ui.Mvvm.Contracts;
using Wpf.Ui.Controls.Interfaces;

namespace NectarRCON.Services;
public class RconConnectService : IRconConnectService
{
    private readonly IServerPasswordService _serverPasswordService;
    private readonly ILanguageService _languageService;

    public RconConnectService(IServerPasswordService serverPasswordService, ILanguageService languageService)
    {
        _serverPasswordService = serverPasswordService;
        _languageService = languageService;
    }

    private RCON? _rconClient
        = null;
    private bool _connecting = false;
    private bool _connected = false;

    public event MessageEvent? OnMessage;
    public event ClosedEvent? OnClosed;
    public event EventEvent? OnConnected;
    public event EventEvent? OnConnecting;

    private ServerInformation _serverInformation = new();
    public void Close()
    {
        lock (this)
        {
            if (IsConnected() && !IsConnecting())
            {
                _connected = false;
                _connecting = false;
                _rconClient?.Dispose();
                _rconClient = null;
                OnClosed?.Invoke(_serverInformation);
            }
        }
    }
    public async Task ConnectAsync(ServerInformation info)
    {
        _connecting = true;
        OnConnecting?.Invoke(info);
        try
        {
            if (IsConnected() && _rconClient != null)
                Close();
            string address = info.Address;
            if (address.ToLower() == "localhost")
                address = "127.0.0.1";
            string password = string.Empty;
            var serverPassword = _serverPasswordService.Get(info);
            if (null != serverPassword)
            {
                password = serverPassword.Password;
            }
            _rconClient = new(IPAddress.Parse(address), info.Port, password, 10000);
            await _rconClient.ConnectAsync();
            _connected = true;
            OnConnected?.Invoke(info);
            _serverInformation = info;
            _rconClient.OnDisconnected += () =>
            {
                OnClosed?.Invoke(_serverInformation);
                _connected = false;
            };
        }
        catch (FormatException ex)
        {
            MessageBox.Show(_languageService.GetKey("ui.text.format_exception")
                .Replace("%s", ex.Message), _languageService.GetKey("text.error"), MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            _connecting = false;
        }
    }
    public bool IsConnected()
        => _connected;
    public bool IsConnecting()
        => _connecting;
    public async Task Send(string command)
    {
        if (IsConnected() && _rconClient != null)
        {
            try
            {
                string result = await _rconClient.SendCommandAsync(command) ?? string.Empty;
                OnMessage?.Invoke(_serverInformation, result);
            }
            catch (Exception ex)
            {
                Close();
                MessageBox.Show($"{_languageService.GetKey("text.error")}\n{ex.Message}", ex.GetType().FullName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        else
        {
            Close();
            MessageBox.Show($"{_languageService.GetKey("text.server.not_connect.text")}", _languageService.GetKey("text.error"), MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
