using NectarRCON.Adapter.Minecraft;
using NectarRCON.Export.Interfaces;

namespace NectarRCON.Tests.Adapter
{
    [TestClass]
    public class MinecraftRconTest
    {
        private static readonly IRconAdapter _client = new MinecraftRconClient();
        private bool _isAuthenticated = false;

        /// <summary>
        /// 此单元测试请自行搭建服务器测试(没必要=3=)
        /// </summary>

        private void connectAndAuthenticate()
        {
            if (!_client.IsConnected)
            {
                _client.Connect("127.0.0.1", 25575);
                _isAuthenticated = _client.Authenticate("123");
            }
        }

        //[TestMethod]
        public void Connect()
        {
            connectAndAuthenticate();
            Assert.IsTrue(_client.IsConnected);
        }

        //[TestMethod]
        public void Authenticate()
        {
            connectAndAuthenticate();
            Assert.IsTrue(_isAuthenticated);
        }

        //[TestMethod]
        public void Run()
        {
            connectAndAuthenticate();
            string response = _client.Run("list");
            Console.WriteLine(response);
            Assert.IsTrue(response != string.Empty);

            _client.Run("say 你好"); // UTF-8 测试
        }

    }
}
