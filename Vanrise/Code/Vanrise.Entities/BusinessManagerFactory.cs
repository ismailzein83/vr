using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public static class EntitiesBusinessManagerFactory
    {
        static EntitiesObjectFactory s_objectFactory;
        static EntitiesBusinessManagerFactory()
        {
            s_objectFactory = new EntitiesObjectFactory(Assembly.Load("Vanrise.Common.Business"));
        }

        public static T GetManager<T>() where T : class, IBEManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
