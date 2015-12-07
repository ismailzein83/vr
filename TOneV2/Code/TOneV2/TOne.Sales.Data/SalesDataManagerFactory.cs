using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace TOne.Sales.Data
{
    public static class SalesDataManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static SalesDataManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("TOne.Sales.Data.SQL"));
        }
        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
