using NectarRCON.Adapter.Minecraft;
using NectarRCON.Adapter.PalWorld;
using NectarRCON.Export.Interfaces;

namespace NectarRCON.Core.Helper
{
    public static class AdapterHelpers
    {
        private static readonly Dictionary<string, Type> AdapterMapping = new()
        {
            {
                string.Empty, typeof(MinecraftRconClient)
            },
            {
                "rcon.minecraft", typeof(MinecraftRconClient)
            },
            {
                "rcon.palworld", typeof(PalWorldRconClient)
            }
        };

        public static IRconAdapter? CreateAdapterInstance(string adapter)
        {
            if (AdapterMapping.TryGetValue(adapter.ToLower(), out var adapterType))
            {
                return Activator.CreateInstance(adapterType) as IRconAdapter;
            }

            return null;
        }
    }
}