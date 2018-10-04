using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.RDB.Tests
{
    public static class SQLDataManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static SQLDataManagerFactory()
        {
            s_objectFactory = new ObjectFactory
                (
                    Assembly.Load("Vanrise.Invoice.Data.SQL"), 
                    Assembly.Load("Vanrise.AccountBalance.Data.SQL")
                );
        }

        public static T GetDataManager<T>() where T : class
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
