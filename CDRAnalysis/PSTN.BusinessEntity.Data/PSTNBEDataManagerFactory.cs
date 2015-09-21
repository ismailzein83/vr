using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace PSTN.BusinessEntity.Data
{
    public static class PSTNBEDataManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static PSTNBEDataManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("PSTN.BusinessEntity.Data.SQL"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
