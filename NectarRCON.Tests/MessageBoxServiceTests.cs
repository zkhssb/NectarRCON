using NectarRCON.Interfaces;
using NectarRCON.Services;
using System.Windows;

namespace NectarRCON.Tests
{
    [TestClass]
    public class MessageBoxServiceTests
    {
        private class MyLanguageService : ILanguageService
        {
            public string GetKey(string key)
            {
                return key;
            }

            public Dictionary<string, ResourceDictionary> GetLanguages()
            {
                throw new NotImplementedException();
            }

            public ResourceDictionary GetSelectedLanguage()
            {
                throw new NotImplementedException();
            }

            public void Refresh()
            {
                throw new NotImplementedException();
            }

            public void SelectLanguage()
            {
                throw new NotImplementedException();
            }

            public void SelectLanguage(string languageName, bool name)
            {
                throw new NotImplementedException();
            }
        }

        private readonly IMessageBoxService _service = new MessageBoxService(new MyLanguageService());
        [TestMethod]
        public void ExceptionTest()
        {
            _service.Show(new Exception("测试异常"), "我故意哒!");
        }
    }
}
