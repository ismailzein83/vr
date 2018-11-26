using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Retail.Demo.Data
{
    public class RetailDemoDataManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static RetailDemoDataManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("Retail.Demo.Data.SQL"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
