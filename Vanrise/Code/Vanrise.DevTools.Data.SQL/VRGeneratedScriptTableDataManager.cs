using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.DevTools.Data;
using Vanrise.DevTools.Entities;

namespace Vanrise.DevTools.Data.SQL
{
    public class TableDataManager : BaseSQLDataManager, IVRGeneratedScriptTableDataManager
    {
        public TableDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }

        public string Connection_String { get; set; }

        protected override string GetConnectionString()
        {
            return Connection_String;
        }

        public List<VRGeneratedScriptTable> GetTables(string schemaName)
        {

            return GetItemsText("select sys.tables.name from sys.tables inner join sys.schemas on sys.tables.schema_id=sys.schemas.schema_id WHERE sys.schemas.name='" + schemaName + "'", TableMapper, null);
        }

        VRGeneratedScriptTable TableMapper(IDataReader reader)
        {
            return new VRGeneratedScriptTable
            {
                Name = GetReaderValue<string>(reader, "name"),

            };
        }
    }
}
