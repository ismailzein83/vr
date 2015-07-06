using System.Reflection;
using Vanrise.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Integration.Data
{
    public static class IntegrationDataManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static IntegrationDataManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("Vanrise.Integration.Data.SQL"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
