using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Entities
{
    public static class ContextFactory
    {
        static ObjectFactory s_objectFactory;
        static ContextFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("TOne.WhS.BusinessEntity.Business"));
        }

        public static T CreateContext<T>() where T : class, IContext
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
