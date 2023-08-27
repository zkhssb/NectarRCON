using NectarRCON.Interfaces;
using NectarRCON.Models;
using NectarRCON.Services;
using System.Windows;

namespace NectarRCON.Tests
{
    [TestClass]
    public class GroupServiceTests
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
        private readonly IGroupService _groupService = new GroupService(new MessageBoxService(new MyLanguageService()), new MyLanguageService());

        [TestMethod]
        public void Dump()
        {
            _groupService.GetGroups();
        }

        [TestMethod]
        public void TestAll()
        {
            Group group = new Group()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test",
                Servers = new(),
            };
            _groupService.Add(group);
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                _groupService.Add(group); // 相同的Id 和Name
            });
            Assert.IsTrue(File.Exists("./groups/" + group.Id + ".json"));
            _groupService.Delete(group.Id);
            Assert.IsFalse(File.Exists("./groups/" + group.Id + ".json"));
            Assert.IsNull(_groupService.FindGroup(group.Name));
            Assert.IsNull(_groupService.GetGroup(group.Id));
            Assert.IsTrue(_groupService.GetGroups().Count == 0);
        }
    }
}
