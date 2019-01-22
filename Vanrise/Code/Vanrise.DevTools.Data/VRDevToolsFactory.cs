using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.DevTools.Data
{
    public class VRDevToolsFactory
    {
        static ObjectFactory s_objectFactory;
        static VRDevToolsFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("Vanrise.DevTools.Data.SQL"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
