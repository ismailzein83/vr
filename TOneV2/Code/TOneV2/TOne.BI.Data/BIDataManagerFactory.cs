using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace TOne.BI.Data
{   

    public static class BIDataManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static BIDataManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("TOne.BI.Data.SQL"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
