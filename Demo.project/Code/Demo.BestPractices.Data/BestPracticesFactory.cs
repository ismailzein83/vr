using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Demo.BestPractices.Data
{
   public class BestPracticesFactory
    {
         static ObjectFactory s_objectFactory;
         static BestPracticesFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("Demo.BestPractices.Data.SQL"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
