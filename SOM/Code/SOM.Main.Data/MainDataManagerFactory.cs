using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace SOM.Main.Data
{
    public static class MainDataManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static MainDataManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("SOM.Main.Data.SQL"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
