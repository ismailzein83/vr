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

        VRDictionary<Type, Type> _cachedImplementedTypes = new VRDictionary<Type, Type>(true);

        static VRDictionary<Type, Type> s_explicitImplementedTypes = new VRDictionary<Type, Type>(true);

        public static void AddExplicitImplementation<T, Q>()
        {
            s_explicitImplementedTypes.Add(typeof(T), typeof(Q));
        }

        public static void RemoveExplicitImplementation<T>()
        {            
            s_explicitImplementedTypes.Remove(typeof(T));
        }

        public T CreateObjectFromType<T>() where T : class
        {
            Type implementationType = GetCachedType<T>();
            return Activator.CreateInstance(implementationType) as T;
        }

        private Type GetCachedType<T>()
        {            
            Type implementationType;
            Type baseType = typeof(T);
            if (s_explicitImplementedTypes.TryGetValue(baseType, out implementationType))
                return implementationType;
            if (!_cachedImplementedTypes.TryGetValue(baseType, out implementationType))
            {
                implementationType = GetImplementationType(baseType);
                _cachedImplementedTypes.TryAdd(baseType, implementationType);
            }
            return implementationType;            
        }

        private Type GetImplementationType(Type baseType)
        {
            foreach (var a in _assemblies)
            {
                foreach (Type t in a.GetLoadableTypes())
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
