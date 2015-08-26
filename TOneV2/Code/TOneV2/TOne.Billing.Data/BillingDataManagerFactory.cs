using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace TOne.Billing.Data
{
    public static class BillingDataManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static BillingDataManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("TOne.Billing.Data.SQL"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
