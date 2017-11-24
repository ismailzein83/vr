using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    internal class EntitiesObjectFactory
    {
        Assembly[] _assemblies;
        public EntitiesObjectFactory(params Assembly[] assemblies)
        {
            _assemblies = assemblies;
        }

        ConcurrentDictionary<Type, Type> _cachedImplementedTypes = new ConcurrentDictionary<Type, Type>();

        static ConcurrentDictionary<Type, Type> s_explicitImplementedTypes = new ConcurrentDictionary<Type, Type>();

        public static void AddExplicitImplementation<T, Q>()
        {
            if (!s_explicitImplementedTypes.TryAdd(typeof(T), typeof(Q)))
                throw new Exception(String.Format("Cannot add Item to dictionary. Item Key '{0}'", typeof(T)));
        }

        public static void RemoveExplicitImplementation<T>()
        {
            Type type;
            s_explicitImplementedTypes.TryRemove(typeof(T),out type);
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
                foreach (Type t in GetLoadableTypes(a))
                {
                    if (baseType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                    {
                        return t;
                    }
                }
            }
            throw new ArgumentException(String.Format("Could not find implementation of the base type '{0}'", baseType));
        }

        private IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }

    }
}
