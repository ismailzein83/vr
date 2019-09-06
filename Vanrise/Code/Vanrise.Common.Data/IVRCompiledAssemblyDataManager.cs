using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface IVRCompiledAssemblyDataManager : IDataManager
    {
        void AddAssembly(Guid assemblyId, string name, Guid devProjectId, byte[] assemblyContent, DateTime compiledTime);

        VRCompiledAssembly GetAssemblyByName(string assemblyName);

        List<VRCompiledAssembly> GetAssembliesEffectiveForDevProjects();
    }
}
