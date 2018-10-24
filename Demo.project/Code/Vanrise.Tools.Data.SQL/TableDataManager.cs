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
            base(GetConnectionStringName("", ""))
        {
        }

        public string Connection_String { get; set; }


        protected override string GetConnectionString()
        {
            return Connection_String;
        }
         
        public List<Table> GetTables()
        {

            return GetItemsText("SELECT *FROM sys.Tables", TableMapper,null);
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
