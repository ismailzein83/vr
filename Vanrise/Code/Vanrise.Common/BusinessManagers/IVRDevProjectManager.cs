using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common
{
    public interface IVRDevProjectManager : IBEManager
    {
        Dictionary<Guid, VRDevProject> GetCachedVRDevProjects();
    }
}
