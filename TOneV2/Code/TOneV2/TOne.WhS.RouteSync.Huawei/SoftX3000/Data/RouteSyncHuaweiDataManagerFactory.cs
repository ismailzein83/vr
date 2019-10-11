using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace TOne.WhS.RouteSync.Huawei.SoftX3000.Data
{
    public static class RouteSyncHuaweiDataManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static RouteSyncHuaweiDataManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("TOne.WhS.RouteSync.Huawei.RDB"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
