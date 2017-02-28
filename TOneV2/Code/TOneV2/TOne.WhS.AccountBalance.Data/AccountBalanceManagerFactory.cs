using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace TOne.WhS.AccountBalance.Data
{
    public class AccountBalanceManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static AccountBalanceManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("TOne.WhS.AccountBalance.Data.SQL"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
