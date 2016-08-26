using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common
{
    public static class BusinessManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static BusinessManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("Vanrise.Common.Business"));
        }

        public static T GetManager<T>() where T : class, IBEManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
