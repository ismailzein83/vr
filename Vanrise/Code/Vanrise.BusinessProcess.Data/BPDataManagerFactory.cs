using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Common;
using System.Reflection;

namespace Vanrise.BusinessProcess.Data
{
    public static class BPDataManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static BPDataManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("Vanrise.BusinessProcess.Data.SQL"));
        }

        public static T GetDataManager<T>() where T : class
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
