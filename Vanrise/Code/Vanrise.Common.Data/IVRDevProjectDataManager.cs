using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface IVRDevProjectDataManager : IDataManager
    {
        List<VRDevProject> GetDevProjects();

        bool AreVRDevProjectUpdated(ref object updateHandle);

        void UpdateProjectAssemblyId(Guid devProjectId, Guid assemblyId, bool updateProjectOnlyIfExistingAssemblyNotChanged, Guid? existingAssemblyId);
    }
}
