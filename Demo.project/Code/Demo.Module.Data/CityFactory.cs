using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Demo.Module.Data
{
    public class CityFactory 
    {
         static ObjectFactory s_objectFactory;
         static CityFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("Demo.Module.Data.SQL"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
