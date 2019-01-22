using System.Collections.Generic;
using Vanrise.DevTools.Entities;

namespace Vanrise.DevTools.Data
{
    public interface IVRGeneratedScriptTableDataManager : IDataManager
    {

        List<VRGeneratedScriptTable> GetTables(string schemaName);
        string Connection_String { set; }


    }
}
