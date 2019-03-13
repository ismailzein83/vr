using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Retail.Interconnect.Data
{
    public class InterconnectInvoiceManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static InterconnectInvoiceManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("Retail.Interconnect.Data.RDB"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
