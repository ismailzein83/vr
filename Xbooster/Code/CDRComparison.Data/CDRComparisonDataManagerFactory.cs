using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace CDRComparison.Data
{
    public static class CDRComparisonDataManagerFactory
    {
        private static ObjectFactory _objectFactory;

        static CDRComparisonDataManagerFactory()
        {
            _objectFactory = new ObjectFactory(Assembly.Load("CDRComparison.Data.SQL"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return _objectFactory.CreateObjectFromType<T>();
        }
    }
}
