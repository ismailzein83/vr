using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Runtime.Entities
{
    public static class RuntimeFactory
    {
        static ObjectFactory s_objectFactory;
        static RuntimeFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("Vanrise.Runtime"));
        }

        public static IRunningProcessManager GetRunningProcessManager()
        {
            return s_objectFactory.CreateObjectFromType<IRunningProcessManager>();
        }
    }
}
