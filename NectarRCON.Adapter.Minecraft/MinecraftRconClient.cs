using NectarRCON.Export.Client;
using NectarRCON.Export.Interfaces;
using System.ComponentModel;
using System.Text;

namespace NectarRCON.Adapter.Minecraft
{
    [Description("rcon.minecraft")]
    public class MinecraftRconClient : BaseTcpClient, IRconAdapter
    {
        // https://wiki.vg/RCON #Fragmentation
        private static readonly int MaxMessageSize = 4110;

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
            _semaphore.Wait();
            try
            {
                Packet response = Send(new Packet(PacketType.Command, command));
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
                Packet packet = Send(new Packet(PacketType.Authenticate, password));
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
            return PacketEncoder.Decode(Send(packet.Encode(_encoding)), _encoding);
        }
    }
}