using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http.Dispatcher;

namespace Vanrise.Web
{
    public class VRAssembliesResolver : DefaultAssembliesResolver
    {
        public override ICollection<Assembly> GetAssemblies()
        {
            List<Assembly> assemblies = new List<Assembly>(System.Web.Compilation.BuildManager.GetReferencedAssemblies().OfType<Assembly>());
            return assemblies;
        }
    }
}