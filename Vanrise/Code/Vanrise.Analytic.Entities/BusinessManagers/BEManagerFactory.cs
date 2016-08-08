using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Analytic.Entities
{
    public static class BEManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static BEManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("Vanrise.Analytic.Business"));
        }

        public static T GetManager<T>() where T : class, IBEManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
