using System.Reflection;
using Vanrise.Common;

namespace CP.SupplierPricelist.Data
{
    public class CustomerDataManagerFactory
    {
        private static ObjectFactory _objectFactory;

        static CustomerDataManagerFactory()
        {
            _objectFactory = new ObjectFactory(Assembly.Load("CP.SupplierPricelist.Data.SQL"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return _objectFactory.CreateObjectFromType<T>();
        }
    }
}
