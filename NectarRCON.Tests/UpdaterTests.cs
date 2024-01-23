using NectarRCON.Updater;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NectarRCON.Tests
{
    [TestClass]
    public class UpdaterTests
    {
        [TestMethod]
        public void Github()
        {
            IUpdater updater = new GithubUpdater();
            updater.SetVersion("NectarRcon-x86-1.0.0");
            updater.IsLatestVersion();
        }

        [TestMethod]
        public void AppVersionTest()
        {
            AppVersion versionA = AppVersion.ParseVersion("TestApp-x64-1.0.0-beta1");
            AppVersion versionB = AppVersion.ParseVersion("TestApp-x64-1.0.0-beta2");

            Assert.IsTrue(versionA.Equals(versionA));
            Assert.IsFalse(versionA.Equals(versionB));

#pragma warning disable CS1718 // 对同一变量进行了比较
            Assert.IsTrue(versionA == versionA);
            Assert.IsFalse(versionA != versionA);
            Assert.IsFalse(versionA > versionA);
#pragma warning restore CS1718 // 对同一变量进行了比较

            Assert.IsTrue(versionB > versionA);
            Assert.IsFalse(versionB < versionA);
        }
    }
}
