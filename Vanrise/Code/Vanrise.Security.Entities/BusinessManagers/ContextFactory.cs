using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Security.Entities
{
    public static class ContextFactory
    {
        static ObjectFactory s_objectFactory;
        static ContextFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("Vanrise.Security.Business"));
        }

        public static ISecurityContext GetContext()
        {
            return s_objectFactory.CreateObjectFromType<ISecurityContext>();
        }
    }
}
