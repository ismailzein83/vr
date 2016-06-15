using System;
using System.Reflection;
using Vanrise.Common;

namespace Mediation.Generic.Data
{
    public class MediationGenericDataManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static MediationGenericDataManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("Mediation.Generic.Data.SQL"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
