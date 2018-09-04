using System.Reflection;
using Vanrise.Common;

namespace Vanrise.Reprocess.Entities
{
    public static class ReprocessManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static ReprocessManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("Vanrise.Reprocess.Business"));
        }

        public static T GetManager<T>() where T : class, IReprocessManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
