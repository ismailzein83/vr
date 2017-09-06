using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Mediation.Generic.Entities
{
    public static class MediationManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static MediationManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("Mediation.Generic.Business"));
        }

        public static T GetManager<T>() where T : class, IMediationManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
