using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Concurrent;

namespace Vanrise.Common
{
    public class ObjectFactory
    {
        Assembly[] _assemblies;
        public ObjectFactory(params Assembly[] assemblies)
        {
            _assemblies = assemblies;
        }

        ConcurrentDictionary<Type, Object> _cachedObjects = new ConcurrentDictionary<Type, Object>();

        public T CreateObjectFromType<T>() where T : class
        {
            return GetCachedObj<T>() as T;
        }

        private Object GetCachedObj<T>()
        {
            Object obj;
            Type baseType = typeof(T);
            if (!_cachedObjects.TryGetValue(baseType, out obj))
            {
                var implementationType = GetImplementationType(baseType);
                obj = Activator.CreateInstance(implementationType);
                _cachedObjects.TryAdd(baseType, obj);
            }
            return obj;
        }

        private Type GetImplementationType(Type baseType)
        {
            foreach (var a in _assemblies)
            {
                foreach (Type t in a.GetTypes())
                {
                    if (baseType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                    {
                        return t;
                    }
                }
            }
            throw new ArgumentException(String.Format("Could not find implementation of the base type '{0}'", baseType));
        }

        
    }
}
