using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace NP.IVSwitch.Data
{
    public static class IVSwitchDataManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static IVSwitchDataManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("NP.IVSwitch.Data.Postgres"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
