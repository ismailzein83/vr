using System.Reflection;
using Vanrise.Common;

namespace Retail.EntitiesMigrator.Data
{
    public class EntitiesMigratorDataManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static EntitiesMigratorDataManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("Retail.EntitiesMigrator.Data.SQL"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
