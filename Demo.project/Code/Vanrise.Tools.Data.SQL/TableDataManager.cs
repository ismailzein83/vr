using Vanrise.Tools.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Vanrise.Data.SQL;

namespace Vanrise.Tools.Data.SQL
{
    public class TableDataManager : BaseSQLDataManager, ITableDataManager
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
         
        public List<Table> GetTables(string schemaName)
        {

            return GetItemsText("select sys.tables.name from sys.tables inner join sys.schemas on sys.tables.schema_id=sys.schemas.schema_id WHERE sys.schemas.name='"+schemaName+"'", TableMapper, null);
        }
        
        Table TableMapper(IDataReader reader)
        {
            return new Table
            {
                Name = GetReaderValue<string>(reader, "name"),

            };
        }
    }
}
