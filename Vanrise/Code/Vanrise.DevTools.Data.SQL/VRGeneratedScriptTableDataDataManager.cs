using System.Collections.Generic;
using System.Data;
using Vanrise.DevTools.Data;
using Vanrise.Data.SQL;
using Vanrise.DevTools.Entities;
namespace Vanrise.DevTools.Data.SQL
{
    public class TableDataDataManager : BaseSQLDataManager, IVRGeneratedScriptTableDataDataManager
    {
        public TableDataDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }

        public string Connection_String { get; set; }

        protected override string GetConnectionString()
        {
            return Connection_String;
        }
        string table_Name;
        public List<GeneratedScriptItemTableRow> GetTableData(string schemaName, string tableName,string joinCondition ,string whereCondition)
        {
            table_Name = tableName;
            return GetItemsText(string.Format("select * from [{0}].[{1}] as MainTable {2} {3}", schemaName,tableName,joinCondition,(whereCondition != null && whereCondition.Length > 0) ? string.Format("where {0}", whereCondition) : null), TableDataMapper, null);

        }


        GeneratedScriptItemTableRow TableDataMapper(IDataReader reader)
        {
            IVRGeneratedScriptColumnsDataManager columnsDataManager = VRDevToolsFactory.GetDataManager<IVRGeneratedScriptColumnsDataManager>();

            columnsDataManager.Connection_String = Connection_String;

            List<VRGeneratedScriptColumns> columns = columnsDataManager.GetColumns(table_Name);
            GeneratedScriptItemTableRow tableData = new GeneratedScriptItemTableRow();
            tableData.FieldValues = new Dictionary<string, object>();


            foreach (var column in columns)
            {
                if (column.Name != "timestamp")
                    tableData.FieldValues.Add(column.Name, GetReaderValue<dynamic>(reader, column.Name));
            }

            return tableData;
        }
    }
}
