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

        public List<SourceCode> GetSourceCodes()
        {
            return GetItemsText(query_getSourceCodes, SourceCodeMapper, null);
        }

        private SourceCode SourceCodeMapper(IDataReader arg)
        {
            return new SourceCode()
            {
                SourceId = arg["ID"].ToString(),
                ZoneId = GetReaderValue<int>(arg, "ZoneID"),
                Code = arg["Code"] as string,
                BeginEffectiveDate = (DateTime)arg["BeginEffectiveDate"],
                EndEffectiveDate = GetReaderValue<DateTime?>(arg, "EndEffectiveDate"),
            };
        }

        const string query_getSourceCodes = @"SELECT [ID]  ,[Code]  ,[ZoneID]  ,[BeginEffectiveDate]  ,[EndEffectiveDate]  FROM [dbo].[Code] WITH (NOLOCK)";
    }
}
