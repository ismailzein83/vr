using System;
using Vanrise.Common;
using System.Reflection;

namespace NP.IVSwitch.Entities
{
    public class NPManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static NPManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("NP.IVSwitch.Business"));
        }

        public static T GetManager<T>() where T : class, IBEManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
