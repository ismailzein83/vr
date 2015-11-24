using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace TOne.WhS.Analytics.Data
{
   public  class AnalyticsDataManagerFactory
    {
           static ObjectFactory s_objectFactory;
           static AnalyticsDataManagerFactory()
           {
               s_objectFactory = new ObjectFactory(Assembly.Load("TOne.WhS.Analytics.Data.SQL"));
           }

           public static T GetDataManager<T>() where T : class, IDataManager
           {
               return s_objectFactory.CreateObjectFromType<T>();
           }

    }
}
