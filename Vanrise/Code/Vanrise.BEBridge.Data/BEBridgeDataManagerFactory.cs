using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.BEBridge.Data
{
    public static class BEBridgeDataManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static BEBridgeDataManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("Vanrise.BEBridge.Data.SQL"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
