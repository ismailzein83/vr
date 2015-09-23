using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Data;

namespace TOne.WhS.SupplierPriceList.Data
{
   public class SupPLDataManagerFactory
    {
          static ObjectFactory s_objectFactory;
          static SupPLDataManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("TOne.WhS.SupplierPriceList.Data.SQL"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
