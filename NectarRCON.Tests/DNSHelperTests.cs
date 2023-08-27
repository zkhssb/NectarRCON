using NectarRCON.Core.Helper;
namespace NectarRCON.Tests
{
    [TestClass]
    public class DNSHelperTests
    {
        /// <summary>
        /// A解析测试
        /// </summary>
        [TestMethod]
        public void ATest()
        {
            string? ip = DNSHelpers.AQuery("hypixel.net");
            Console.WriteLine(ip);
            Assert.IsNotNull(string.IsNullOrEmpty(ip));
        }

        /// <summary>
        /// SRV解析测试
        /// </summary>
        [TestMethod]
        public void SRVTest()
        {
            string? ip = DNSHelpers.SRVQuery("mc.125nf.com");
            Console.WriteLine(ip);
            Assert.IsFalse(string.IsNullOrEmpty(ip));
        }
    }
}
