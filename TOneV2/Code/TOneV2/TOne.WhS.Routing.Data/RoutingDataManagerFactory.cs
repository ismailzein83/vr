using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace TOne.WhS.Routing.Data
{
    public static class RoutingDataManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static RoutingDataManagerFactory()
        {
            if (ConfigurationManager.AppSettings["Routing_TOneV1"] == "true")
                s_objectFactory = new ObjectFactory(Assembly.Load("TOne.Whs.Routing.Data.TOneV1SQL"));
            else
                s_objectFactory = new ObjectFactory(Assembly.Load("TOne.WhS.Routing.Data.SQL"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
