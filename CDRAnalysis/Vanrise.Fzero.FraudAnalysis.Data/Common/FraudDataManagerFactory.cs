using System.Reflection;
using Vanrise.Common;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public static class FraudDataManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static FraudDataManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("Vanrise.Fzero.FraudAnalysis.Data.SQL"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
