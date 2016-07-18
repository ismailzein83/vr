using Vanrise.Data.SQL;

namespace Vanrise.HelperTools.Data
{
    public class HelperToolDataManager : BaseSQLDataManager
    {
        public HelperToolDataManager(string connectionstring)
            : base(connectionstring, false)
        {

        }

        public void BulkInsert(string tableName, string rowData, string columnNames)
        {
            var stream = base.InitializeStreamForBulkInsert();
            stream.WriteRecord(rowData);
            stream.Close();

            base.InsertBulkToTable(new Vanrise.Data.SQL.StreamBulkInsertInfo
            {
                TableName = tableName,
                Stream = stream,
                TabLock = false,
                FieldSeparator = '^',
                KeepIdentity = true,
                ColumnNames = columnNames.Split(',')
            });
        }
    }
}
