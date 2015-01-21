using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Common;
using System.Reflection;

namespace TOne.LCR.Data
{
    public static class LCRDataManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static LCRDataManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("TOne.LCR.Data.SQL"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
