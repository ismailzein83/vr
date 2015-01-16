using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TABS.Components
{
    public  static class InterfaceChekerManager
    {
        public static bool IsImplementationOf(this Type checkMe, Type forMe)
        {
            foreach (Type iface in checkMe.GetInterfaces())
                if (iface == forMe)
                    return true;
            return false;
        }
    }
}
