using NectarRCON.Adapter.Minecraft;
using NectarRCON.Export.Interfaces;
namespace NectarRCON.Core.Helper
{
    public static class AdapterHelpers
    {
        public static IRconAdapter? CreateAdapterInstance(string adapter)
        {
            return new MinecraftRconClient(); // 暂时这么写
        }
    }
}