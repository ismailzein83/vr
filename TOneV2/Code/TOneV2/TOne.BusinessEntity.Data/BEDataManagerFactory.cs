using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace TOne.BusinessEntity.Data
{
    public static class BEDataManagerFactory
    { 
        static ObjectFactory s_objectFactory;
        static BEDataManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("TOne.BusinessEntity.Data.SQL"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
