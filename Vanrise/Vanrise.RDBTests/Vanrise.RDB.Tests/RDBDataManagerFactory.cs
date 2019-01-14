using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.RDB.Tests
{
    public static class RDBDataManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static RDBDataManagerFactory()
        {
            s_objectFactory = new ObjectFactory
                (
                    Assembly.Load("Vanrise.Invoice.Data.RDB"),
                    Assembly.Load("Vanrise.AccountBalance.Data.RDB"),
                    Assembly.Load("Vanrise.Rules.Data.RDB"),
                    Assembly.Load("Vanrise.Common.Data.RDB"),
                    Assembly.Load("Vanrise.GenericData.Data.RDB"),
                    Assembly.Load("Vanrise.GenericData.Transformation.Data.RDB")


                );
        }

        public static T GetDataManager<T>() where T : class
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
