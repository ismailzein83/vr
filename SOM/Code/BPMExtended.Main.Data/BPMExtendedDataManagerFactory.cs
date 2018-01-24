using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace BPMExtended.Main.Data
{
    public static class BPMExtendedDataManagerFactory
    { 
        static ObjectFactory s_objectFactory;
        static BPMExtendedDataManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("BPMExtended.Main.Data.SQL"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
