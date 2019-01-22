using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.DevTools.Data;
using Vanrise.DevTools.Entities;

namespace Vanrise.DevTools.Data.SQL
{
    public class ColumnsDataManager : BaseSQLDataManager, IVRGeneratedScriptColumnsDataManager
    {
        public ColumnsDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }

        public string Connection_String { get; set; }

        protected override string GetConnectionString()
        {
            return Connection_String;
        }
        public List<VRGeneratedScriptColumns> GetColumns(string tableName)
        {
            return GetItemsText("select cols.name from sys.columns cols inner join sys.tables tbls on cols.object_id= tbls.object_id WHERE tbls.name='" + tableName + "'", ColumnsMapper, null);
        }


        VRGeneratedScriptColumns ColumnsMapper(IDataReader reader)
        {
            return new VRGeneratedScriptColumns
            {
                Name = GetReaderValue<string>(reader, "name"),

            };
        }
    }
}
