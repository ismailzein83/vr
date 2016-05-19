using System;
using System.Collections.Generic;
using System.Data;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SourceCodeDataManager : BaseSQLDataManager
    {
        public SourceCodeDataManager(string connectionString)
            : base(connectionString, false)
        {
        }

        public List<SourceCode> GetSourceCodes(bool isSaleCode)
        {
            return GetItemsText(query_getSourceCodes + (isSaleCode ? "where Zone.SupplierID = 'SYS'" : ""), SourceCodeMapper, null);
        }

        private SourceCode SourceCodeMapper(IDataReader arg)
        {
            return new SourceCode()
            {
                SourceId = arg["ID"].ToString(),
                ZoneId = GetReaderValue<int>(arg, "ZoneID"),
                Code = arg["Code"] as string,
                CodeGroup = arg["CodeGroup"] as string,
                BeginEffectiveDate = (DateTime)arg["BeginEffectiveDate"],
                EndEffectiveDate = GetReaderValue<DateTime?>(arg, "EndEffectiveDate"),
            };
        }

        const string query_getSourceCodes = @"SELECT Code.ID ID, Code.Code Code, Code.ZoneID ZoneID, 
                                                     Code.BeginEffectiveDate BeginEffectiveDate, Code.EndEffectiveDate EndEffectiveDate, Zone.CodeGroup CodeGroup
                                                     FROM Code WITH (NOLOCK) INNER JOIN Zone WITH (NOLOCK)  ON Code.ZoneID = Zone.ZoneID ";
    }
}
