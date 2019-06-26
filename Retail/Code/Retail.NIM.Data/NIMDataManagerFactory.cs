using System.Reflection;
using Vanrise.Common;

namespace Retail.NIM.Data
{
    public class NIMDataManagerFactory
    {
        static ObjectFactory s_objectFactory;

        static NIMDataManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("Retail.NIM.Data.RDB"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}