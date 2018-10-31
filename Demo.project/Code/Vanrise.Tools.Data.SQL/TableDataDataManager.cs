using Vanrise.Tools.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Data.SQL;
namespace Vanrise.Tools.Data.SQL
{
    public class TableDataDataManager : BaseSQLDataManager, ITableDataDataManager
    {
        public TableDataDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }

        public string table_Name { get; set; }
        public string Connection_String { get; set; }
        
        protected override string GetConnectionString()
        {
            return Connection_String;
        }
        public List<TableData> GetTableData(string schemaName,string tableName,string whereCondition)
        {
            table_Name = tableName;
            return GetItemsText("select * from [" + schemaName + "].[" + tableName + "]" + whereCondition, TableDataMapper, null);
        }


        TableData TableDataMapper(IDataReader reader)
        {
            IColumnsDataManager columnsDataManager = VRToolsFactory.GetDataManager<IColumnsDataManager>();

            columnsDataManager.Connection_String = Connection_String;

            List<Columns> columns= columnsDataManager.GetColumns(table_Name);
            TableData tableData = new TableData();
            tableData.FieldValues = new Dictionary<string, object>();


           foreach (var column in columns)
           {
                tableData.FieldValues.Add(column.Name, GetReaderValue<dynamic>(reader, column.Name));
           }

            return tableData;
        }
    }
}
