using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace QM.BusinessEntity.Data
{
    public class BEDataManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static BEDataManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("QM.BusinessEntity.Data.SQL"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
