using System.Reflection;
using Vanrise.Common;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public static class FraudManagerFactory
    {
        static ObjectFactory s_objectFactory;

        static FraudManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("Vanrise.Fzero.FraudAnalysis.Business"));
        }

        public static T GetManager<T>() where T : class, IBusinessManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
