using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Vanrise.Common;

namespace Retail.Data.Data
{
    public class DataDataManagerFactory
    {
        static ObjectFactory s_objectFactory;

        static DataDataManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("Retail.Data.Data.SQL"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
