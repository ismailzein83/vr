using System;
using System.Reflection;
using Retail.BusinessEntity.Data;
using Vanrise.Common;

namespace NetworkProvision.Data
{
    public class NetworkProvisionDataManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static NetworkProvisionDataManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("Retail.BusinessEntity.Data.SQL"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
