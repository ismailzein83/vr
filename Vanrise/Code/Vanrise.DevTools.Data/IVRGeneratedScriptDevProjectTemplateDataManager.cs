using System.Collections.Generic;
using Vanrise.DevTools.Entities;
using Vanrise.Entities;

namespace Vanrise.DevTools.Data
{
    public interface IVRGeneratedScriptDevProjectTemplateDataManager : IDataManager
    {
        List<VRDevProject> GetDevProjects();
        string Connection_String { set; }
    }
}
