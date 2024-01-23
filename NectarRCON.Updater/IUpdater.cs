namespace NectarRCON.Updater
{
    public interface IUpdater
    {
        /// <summary>
        /// 设置版本
        /// </summary>
        void SetVersion(string version);
        /// <summary>
        /// 是最新版
        /// </summary>
        bool IsLatestVersion();
        /// <summary>
        /// 获取最新版本
        /// </summary>
        AppVersion GetLatestVersion();
        /// <summary>
        /// 开始安装
        /// </summary>
        void Setup();
        /// <summary>
        /// 设置是否启用获取预发布版本更新
        /// </summary>
        void SetPreEnable(bool value);
    }
}
