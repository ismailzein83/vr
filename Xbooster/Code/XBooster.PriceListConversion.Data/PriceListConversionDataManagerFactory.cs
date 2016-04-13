using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace XBooster.PriceListConversion.Data
{
    public class PriceListConversionDataManagerFactory
    {
         static ObjectFactory s_objectFactory;
         static PriceListConversionDataManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("XBooster.PriceListConversion.Data.SQL"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
