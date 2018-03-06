using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace TOne.WhS.Routing.Entities
{
    public static class RoutingManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static RoutingManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("TOne.WhS.Routing.Business"));
        }

        public static T GetManager<T>() where T : class, IRoutingManagerFactory
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
