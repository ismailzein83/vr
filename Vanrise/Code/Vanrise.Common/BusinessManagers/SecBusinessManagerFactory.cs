using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common
{
    internal static class SecBusinessManagerFactory
    {
        static ObjectFactory s_objectFactory;
        static SecBusinessManagerFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("Vanrise.Security.Business"));
        }

        public static T GetManager<T>() where T : class, ISecBusinessManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }

    public interface ISecBusinessManager
    {

    }
}