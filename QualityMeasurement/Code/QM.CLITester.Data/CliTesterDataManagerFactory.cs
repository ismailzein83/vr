using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace QM.CLITester.Data
{
    public class CliTesterDataManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static CliTesterDataManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("QM.CLITester.Data.SQL"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
