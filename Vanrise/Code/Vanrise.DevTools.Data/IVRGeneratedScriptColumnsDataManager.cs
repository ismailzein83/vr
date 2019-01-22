using System.Collections.Generic;
using Vanrise.DevTools.Entities;

namespace Vanrise.DevTools.Data
{
    public interface IVRGeneratedScriptColumnsDataManager : IDataManager
    {

        List<VRGeneratedScriptColumns> GetColumns(string tableName);
        string Connection_String { set; }


    }
}
