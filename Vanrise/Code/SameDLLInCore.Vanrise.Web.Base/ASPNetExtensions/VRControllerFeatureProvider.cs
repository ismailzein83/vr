using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
//using Vanrise.Common;
using System.Linq;

namespace Vanrise.Web.Base
{
    public class VRControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            HashSet<string> addedControllersNames = new HashSet<string>(feature.Controllers.Select(itm => itm.AssemblyQualifiedName));
            foreach (var candidate in GetAllImplementations<BaseAPIController>())
            {
                if (candidate.GetCustomAttribute<System.Web.Http.RoutePrefixAttribute>() != null && !addedControllersNames.Contains(candidate.AssemblyQualifiedName))
                    feature.Controllers.Add(candidate.GetTypeInfo());
            }
        }

        public static IEnumerable<Type> GetAllImplementations(Type baseType)
        {
            string path = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            

            foreach (string fileName in Directory.GetFiles(path, "*.dll"))
            {
                FileInfo info = new FileInfo(fileName);
                Assembly.LoadFile(info.FullName);
            }
            List<Type> lst = new List<Type>();
            List<string> loadedAssemblies = new List<string>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.IsDynamic)
                    continue;
                if (assembly.FullName.Contains("GenericData"))
                    continue;
                if (!assembly.GetName().Name.EndsWith(".Web") && !assembly.GetName().Name.EndsWith(".MainExtensions"))
                    continue;
                if (loadedAssemblies.Contains(assembly.FullName))
                    continue;
                loadedAssemblies.Add(assembly.FullName);
                Type[] types = null;
                try
                {
                    types = assembly.GetExportedTypes();
                }
                catch
                {

                }
                if (types != null)
                {
                    foreach (Type t in types)
                    {
                        if (baseType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                        {
                            lst.Add(t);
                        }
                    }
                }
            }
            return lst;
        }

        public static IEnumerable<Type> GetAllImplementations<T>()
        {
            return GetAllImplementations(typeof(T));
        }
    }
}
