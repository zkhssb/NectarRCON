using System.Text;

namespace NectarRCON.Adapter.PalWorld;
public enum PacketType : int
{
    RespExecCommand = 0,
    ExecCommand = 2,
    ClientAuth = 3
}
public class Packet
{
    public const int HeaderLength = 10;
    
    public readonly int Length;
    public int Id;
    public readonly PacketType Type;
    public readonly string Body;

    public Packet(int length, int id, PacketType type, string body)
    {
        Length = length;
        Id = id;
        Type = type;
        Body = body;
    }

    public Packet(PacketType type, string body)
    {
        Type = type;
        Body = body;
    }

    public void SetId(int id)
    {
        Id = id;
    }

    public byte[] Encode(Encoding? encoding = null)
    {
        List<byte> bytes = new List<byte>();
        var data = (encoding ?? Encoding.UTF8).GetBytes(Body);
        bytes.AddRange(BitConverter.GetBytes(HeaderLength + data.Length));
        bytes.AddRange(BitConverter.GetBytes(Id));
        bytes.AddRange(BitConverter.GetBytes((int)Type));
        bytes.AddRange(data);
        bytes.AddRange(new byte[] { 0, 0 });
        return bytes.ToArray();
    }

    
    public static Packet Decode(byte[] bytes, Encoding? encoding = null)
    {
        if (bytes.Length < HeaderLength)
        {
            throw new ArgumentException("packet length too short");
        }

        var len = BitConverter.ToInt32(bytes, 0);
        var id = BitConverter.ToInt32(bytes, 4);
        var type = BitConverter.ToInt32(bytes, 8);
        var bodyLen = bytes.Length - 12;
        var body = string.Empty;
        if (bodyLen > 0)
        {
            body = (encoding ?? Encoding.UTF8).GetString(bytes, 12, bodyLen);
        }

        return new Packet(len, id, (PacketType)type, body);
    }
}