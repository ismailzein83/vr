using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Voucher.Data
{
    public static class VoucherDataManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static VoucherDataManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("Vanrise.Voucher.Data.SQL"));
        }

        public static T GetDataManager<T>() where T : class, IDataManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
