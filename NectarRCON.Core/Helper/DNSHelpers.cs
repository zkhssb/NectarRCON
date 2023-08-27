using DnsClient;

namespace NectarRCON.Core.Helper
{
    public class DNSHelpers
    {
        public static string AQuery(string host)
        {
            var lookup = new LookupClient();
            var result = lookup.Query(host, QueryType.A);
            var record = result.Answers.ARecords().FirstOrDefault();
            return record?.Address.ToString() ?? string.Empty;
        }

        /// <summary>
        /// MinecraftSRV解析
        /// </summary>
        /// <returns>成功返回ip:port</returns>
        public static string SRVQuery(string host)
        {
            // Minecraft特有的srv
            string srvAddress = $"_minecraft._tcp.{host}";
            var lookup = new LookupClient();
            var result = lookup.Query(srvAddress, QueryType.SRV);
            var record = result.Answers.SrvRecords().FirstOrDefault();
            if (record != null)
            {
                return $"{record.Target.Value}:{record.Port}";
            }
            return string.Empty;
        }
    }
}
