using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Invoice.Entities
{
    public static class BusinessManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static BusinessManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("Vanrise.Invoice.Business"));
        }

        public static T GetManager<T>() where T : class, IBusinessManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }    
}
