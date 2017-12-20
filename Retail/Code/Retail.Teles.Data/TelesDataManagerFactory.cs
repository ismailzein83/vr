using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Retail.Teles.Data
{
    public class TelesDataManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static TelesDataManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("Retail.Teles.Data.SQL"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
