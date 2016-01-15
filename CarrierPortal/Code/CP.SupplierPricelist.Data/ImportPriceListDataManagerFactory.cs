using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace CP.SupplierPricelist.Data
{
    public class ImportPriceListDataManagerFactory
    {
        private static ObjectFactory _objectFactory;

        static ImportPriceListDataManagerFactory()
        {
            _objectFactory = new ObjectFactory(Assembly.Load("CP.SupplierPricelist.Data.SQL"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return _objectFactory.CreateObjectFromType<T>();
        }
    }
}
