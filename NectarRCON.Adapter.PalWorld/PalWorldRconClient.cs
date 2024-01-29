using System.ComponentModel;
using System.Text;
using NectarRCON.Export.Client;
using NectarRCON.Export.Interfaces;

namespace NectarRCON.Adapter.PalWorld;

    [Description("rcon.palworld")]
    public class PalWorldRconClient : BaseTcpClient, IRconAdapter
    {
        private const int MaxMessageSize = 4110;

        private readonly MemoryStream _buffer = new();
        private readonly SemaphoreSlim _semaphore = new(1);
        private int _lastId;
        /// <summary>
        /// 编码
        /// 默认编码 UTF8
        /// </summary>
        private new Encoding _encoding = Encoding.UTF8;

        public void Disconnect()
        {
            _semaphore.Wait();
            try
            {
                Stop();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public override void Dispose()
        {
            _semaphore.Dispose();
            _buffer.Dispose();
            base.Dispose();
        }

        public override byte[] Read(StreamReader reader)
        {
            byte[] buffer = new byte[MaxMessageSize];
            int len = reader.BaseStream.Read(buffer, 0, buffer.Length);
            Array.Resize(ref buffer, len);
            return buffer;
        }

        public string Run(string command)
        {
            if(command.StartsWith("/"))
                command = command[1..];
            _semaphore.Wait();
            try
            {
                var response = Send(new Packet(PacketType.ExecCommand, command));
                return response.Body;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public Encoding GetEncoding()
            => _encoding;

        public void SetEncoding(Encoding encoding)
        {
            _encoding = encoding;
        }

        public IReadOnlyList<string> Commands { get; } = new List<string>()
        {
            "Shutdown {Seconds} {MessageText}",
            "DoExit",
            "KickPlayer {Steam Id}",
            "BanPlayer {Steam Id}",
            "Broadcast {MessageText}",
            "TeleportToPlayer {Steam Id}",
            "TeleportToMe {Steam Id}",
            "ShowPlayers",
            "Info"
        };

        public bool Connect(string address, int port)
        {
            _semaphore.Wait();
            try
            {
                Start(address, port);
                return IsConnected;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public bool Authenticate(string password)
        {
            _semaphore.Wait();
            try
            {
                Packet packet = Send(new Packet(PacketType.ClientAuth, password));
                return packet.Id == _lastId;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private Packet Send(Packet packet)
        {
            Interlocked.Increment(ref _lastId);
            packet.SetId(_lastId);
            return Packet.Decode(Send(packet.Encode(_encoding)), _encoding);
        }
    }