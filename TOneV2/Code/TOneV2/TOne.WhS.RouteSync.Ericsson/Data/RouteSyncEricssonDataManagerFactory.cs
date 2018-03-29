using System;
using System.Reflection;
using Vanrise.Common;

namespace TOne.WhS.RouteSync.Ericsson.Data
{
    public static class RouteSyncEricssonDataManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static RouteSyncEricssonDataManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("TOne.WhS.RouteSync.Ericsson.SQL"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}