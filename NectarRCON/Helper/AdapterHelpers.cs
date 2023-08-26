using NectarRCON.Export.Interfaces;
using System;
using System.Reflection;

namespace NectarRCON.Helper
{
    public static class AdapterHelpers
    {
        public static IRconAdapter? CreateAdapterInstance(string adapter)
        {
            Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            Type? classType = Type.GetType(adapter);
            if (null != classType)
            {
                if (classType.IsSubclassOf(typeof(IRconAdapter)))
                {
                    return Activator.CreateInstance(classType) as IRconAdapter;
                }
            }
            return null;
        }
    }
}