using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.GenericData.Business
{
    public class GenericDataMainExtensionsFactory
    {
          
        static ObjectFactory s_objectFactory;
        static GenericDataMainExtensionsFactory()
        {
            s_objectFactory = new ObjectFactory(Assembly.Load("Vanrise.GenericData.MainExtensions"));
        }

        public static T GetManager<T>() where T : class, IExtensionManager
        {
            return s_objectFactory.CreateObjectFromType<T>();
        }
    }
}
