using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Retail.Cost.Data
{
    public static class CostDataManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static CostDataManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("Retail.Cost.Data.SQL"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
