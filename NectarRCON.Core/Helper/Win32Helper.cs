namespace NectarRCON.Core.Helper;
public static class Win32Helper
{
    public static bool GetWindowsTheme()
    {
        const string RegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        const string RegistryValueName = "AppsUseLightTheme";
        // 这里也可能是LocalMachine(HKEY_LOCAL_MACHINE)
        // see "https://www.addictivetips.com/windows-tips/how-to-enable-the-dark-theme-in-windows-10/"
        object? registryValueObject = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RegistryKeyPath)?.GetValue(RegistryValueName);
        if (registryValueObject is null) return false;
        return (int)registryValueObject > 0 ? false : true;
    }
}