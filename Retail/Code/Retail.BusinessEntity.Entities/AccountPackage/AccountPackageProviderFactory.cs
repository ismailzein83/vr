using System.Reflection;
using Vanrise.Common;

namespace Retail.BusinessEntity.Entities
{
    public class AccountPackageProviderFactory
    {
        static ObjectFactory s_objectFactory;

        static AccountPackageProviderFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("Retail.BusinessEntity.Business"));
        }

        public static T GetManager<T>() where T : class, IAccountPackageProvider
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}