using System.Collections.Generic;
using Vanrise.DevTools.Entities;

namespace Vanrise.DevTools.Data
{
    public interface IVRGeneratedScriptTableDataDataManager : IDataManager
    {
        List<GeneratedScriptItemTableRow> GetTableData(string schemaName, string tableName, string whereCondition);
        string Connection_String { set; }

    }
}
