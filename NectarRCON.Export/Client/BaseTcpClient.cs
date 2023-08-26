using NectarRCON.Export.Interfaces;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NectarRCON.Export.Client;
/// <summary>
/// 简单的TCP客户端实现
/// </summary>
public abstract class BaseTcpClient : IRconClient
{
    private TcpClient? _tcpClient;
    private NetworkStream? _networkStream;
    private StreamReader? _reader;

    protected Encoding? _encoding;

    public bool IsConnected { get; private set; }

    public virtual void Dispose()
    {
        Stop();
    }

    public byte[] Send(byte[] bytes)
    {
        using MemoryStream response = new();
        if(IsConnected)
        {
            _networkStream!.Write(bytes, 0, bytes.Length);
            return Read(_reader!);
        }
        return response.ToArray();
    }

    public abstract byte[] Read(StreamReader reader);

    public void Start(string address, int port)
    {
        if (IsConnected)
        {
            throw new InvalidOperationException("Client is already connected");
        }

        try
        {
            _tcpClient = new(address, port);
            _networkStream = _tcpClient.GetStream();
            _reader = new StreamReader(_networkStream, _encoding ?? Encoding.UTF8);
            IsConnected = true;
        }
        catch
        {
            throw;
        }
    }

    public void Stop()
    {
        if (!IsConnected)
        {
            return;
        }
        _tcpClient?.Dispose();
        _reader?.Dispose();
        IsConnected = false;
    }
}
