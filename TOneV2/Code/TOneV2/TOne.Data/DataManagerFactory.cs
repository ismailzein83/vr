using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Common;
using System.Reflection;

namespace TOne.Data
{
    public static class DataManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static DataManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("TOne.Data.SQL"));
        }

        public static T GetDataManager<T>() where T : class
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
