using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.NumberingPlan.Entities
{
    public static class NPManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static NPManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load(" Vanrise.NumberingPlan.Business"));
        }

        public static T GetManager<T>() where T : class, IBEManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
