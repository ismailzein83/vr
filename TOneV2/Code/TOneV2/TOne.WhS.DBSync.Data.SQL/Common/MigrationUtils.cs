using System;
using Vanrise.Data;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class MigrationUtils
    {
        public static string GetTableName(string schema, string tableName, bool useTempTables)
        {
            return "[" + schema + "].[" + tableName + "" + (useTempTables ? Constants._Temp : "") + "]";
        }

        public static string GetEffectiveQuery(string entityName, bool onlyEffective, DateTime? effectiveFrom)
        {
            string queryOnlyEffective = onlyEffective ? " and {0}.IsEffective = 'Y' " : string.Empty;
            queryOnlyEffective += effectiveFrom.HasValue ? " and {0}.BeginEffectiveDate >= '" + BaseDataManager.GetDateTimeForBCP(effectiveFrom.Value) + "'" : string.Empty;
            return string.Format(queryOnlyEffective, entityName);
        }
    }
}
