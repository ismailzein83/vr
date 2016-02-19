using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace InterConnect.BusinessEntity.Data
{
    public class BEDataManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static BEDataManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("InterConnect.BusinessEntity.Data.SQL"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
