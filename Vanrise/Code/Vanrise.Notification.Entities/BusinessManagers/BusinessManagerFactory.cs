using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Notification.Entities
{
    public static class BusinessManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static BusinessManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("Vanrise.Notification.Business"));
        }

        public static T GetManager<T>() where T : class, IBusinessManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }    
}
