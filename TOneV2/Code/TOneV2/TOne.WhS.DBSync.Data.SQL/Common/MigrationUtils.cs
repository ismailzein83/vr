
namespace TOne.WhS.DBSync.Data.SQL
{
    public class MigrationUtils 
    {
        public static string GetTableName(string schema, string tableName, bool useTempTables)
        {
            return "[" + schema + "].[" + tableName + "" + (useTempTables ? Constants._Temp : "") + "]";
        }
    }
}
