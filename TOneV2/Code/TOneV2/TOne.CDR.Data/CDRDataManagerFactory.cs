using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Common;
using System.Reflection;

namespace TOne.CDR.Data
{
    public static class CDRDataManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static CDRDataManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("TOne.CDR.Data.SQL"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
