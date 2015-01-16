using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Core;
using System.Reflection;

namespace Vanrise.BusinessProcess
{
    internal static class BPDataManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static BPDataManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.GetExecutingAssembly());
        }

        public static T GetDataManager<T>() where T : class
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
