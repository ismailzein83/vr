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

        ConcurrentDictionary<Type, Type> _cachedImplementedTypes = new ConcurrentDictionary<Type, Type>();

        public T CreateObjectFromType<T>() where T : class
        {
            return GetCachedObj<T>() as T;
        }

        private Object GetCachedObj<T>()
        {
            Type implementationType;
            Type baseType = typeof(T);
            if (!_cachedImplementedTypes.TryGetValue(baseType, out implementationType))
            {
                implementationType = GetImplementationType(baseType);
                _cachedImplementedTypes.TryAdd(baseType, implementationType);
            }
            return Activator.CreateInstance(implementationType);
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
