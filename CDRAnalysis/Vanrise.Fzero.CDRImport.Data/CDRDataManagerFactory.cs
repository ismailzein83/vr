using System.Reflection;
using Vanrise.Common;

namespace Vanrise.Fzero.CDRImport.Data
{
    public static class CDRDataManagerFactory
    {
        static ObjectFactory s_objectFactory;

        static CDRDataManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("Vanrise.Fzero.CDRImport.Data.SQL"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
