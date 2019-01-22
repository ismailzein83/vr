using System.Collections.Generic;
using Vanrise.DevTools.Entities;

namespace Vanrise.DevTools.Data
{
    public interface IVRGeneratedScriptSchemaDataManager : IDataManager
    {

        List<VRGeneratedScriptSchema> GetSchemas();
        string Connection_String { set; }


    }
}
