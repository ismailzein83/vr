using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Retail.Ringo.Data
{
    public class RingoDataManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static RingoDataManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("Retail.Ringo.Data.SQL"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
