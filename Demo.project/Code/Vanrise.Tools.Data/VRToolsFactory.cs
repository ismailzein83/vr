using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Tools.Data
{
   public class VRToolsFactory
    {
         static ObjectFactory s_objectFactory;
         static VRToolsFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("Vanrise.Tools.Data.SQL"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
