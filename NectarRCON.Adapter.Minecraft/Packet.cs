using System.Text;

namespace NectarRCON.Adapter.Minecraft;
public class Packet
{
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
        var data = (encoding?? Encoding.UTF8).GetBytes(Body);
        bytes.AddRange(BitConverter.GetBytes(PacketEncoder.HeaderLength + data.Length));
        bytes.AddRange(BitConverter.GetBytes(Id));
        bytes.AddRange(BitConverter.GetBytes((int)Type));
        bytes.AddRange(data);
        bytes.AddRange(new byte[] { 0, 0 });
        return bytes.ToArray();
    }
}
