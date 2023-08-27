using NectarRCON.Models;
using NectarRCON.Rcon;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NectarRCON.Services
{
    /// <summary>
    /// Rcon多客户端连接服务
    /// </summary>
    internal class RconMultiConnection : IRconConnection, IDisposable
    {
        public event MessageEvent? OnMessage;
        public event RconEvent? OnClosed;
        public event RconEvent? OnConnected;
        public event RconEvent? OnConnecting;

        private readonly List<IRconConnection> _connections = new();

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Connect()
        {
            throw new NotImplementedException();
        }

        public bool IsConnected()
        {
            throw new NotImplementedException();
        }

        public bool IsConnecting()
        {
            throw new NotImplementedException();
        }

        public void Send(string command)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            foreach (var connection in _connections)
            {
                connection.Close();
            }
        }
    }
}
