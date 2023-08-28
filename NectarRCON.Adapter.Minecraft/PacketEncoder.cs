using System.Text;

namespace NectarRCON.Adapter.Minecraft
{
    public enum PacketType : int
    {
        Response, // 0: Command response
        _,
        Command, // 2: Command
        Authenticate // 3: Login
    }

    public class PacketEncoder
    {
        public const int HeaderLength = 10;

        public static byte[] Encode(Packet msg)
        {
            List<byte> bytes = new List<byte>();

            bytes.AddRange(BitConverter.GetBytes(msg.Length));
            bytes.AddRange(BitConverter.GetBytes(msg.Id));
            bytes.AddRange(BitConverter.GetBytes((int)msg.Type));
            bytes.AddRange(Encoding.UTF8.GetBytes(msg.Body));
            bytes.AddRange(new byte[] { 0, 0 });

            return bytes.ToArray();
        }

        public static Packet Decode(byte[] bytes)
        {
            if (bytes.Length < HeaderLength) { throw new ArgumentException("packet length too short"); }
            int len = BitConverter.ToInt32(bytes, 0);
            int id = BitConverter.ToInt32(bytes, 4);
            int type = BitConverter.ToInt32(bytes, 8);
            int bodyLen = bytes.Length - (HeaderLength + 4);
            string body = string.Empty;
            if (bodyLen > 0)
            {
                body = Encoding.UTF8.GetString(bytes, 12, bodyLen);
            }
            return new Packet(len, id, (PacketType)type, body);
        }
    }
}
