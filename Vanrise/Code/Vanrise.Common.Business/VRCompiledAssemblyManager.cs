using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.Common.Business
{
    public class VRCompiledAssemblyManager : IVRCompiledAssemblyManager
    {
        static IVRCompiledAssemblyDataManager s_dataManager = CommonDataManagerFactory.GetDataManager<IVRCompiledAssemblyDataManager>();

        static VRCompiledAssemblyManager()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

        }

        private static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith(CSharpCompiler.DEVPROJECT_ASSEMBLY_PREFIX) && args.Name.Contains(CSharpCompiler.DEVPROJECT_ASSEMBLY_SUFFIX))
            {
                var vrAssembly = new VRCompiledAssemblyManager().GetAssemblyByName(args.Name);
                if (vrAssembly != null)
                    return System.Reflection.Assembly.Load(vrAssembly.AssemblyContent);
                else
                    return null;
            }
            else
            {
                return null;
            }
        }

        internal void AddAssembly(Guid assemblyId, string name, Guid devProjectId, byte[] assemblyContent, DateTime compiledTime)
        {
            s_dataManager.AddAssembly(assemblyId, name, devProjectId, assemblyContent, compiledTime);
        }

        private VRCompiledAssembly GetAssemblyByName(string assemblyName)
        {
            return s_dataManager.GetAssemblyByName(assemblyName);
        }

        public List<VRCompiledAssembly> GetAssembliesEffectiveForDevProjects()
        {
            return s_dataManager.GetAssembliesEffectiveForDevProjects();
        }
    }
}
