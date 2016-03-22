using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Analytic.Data
{
    public class AnalyticDataManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static AnalyticDataManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("Vanrise.Analytic.Data.SQL"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
