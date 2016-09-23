using System.Collections.Generic;
using System.Data;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SourceCustomTimeZoneInfoDataManager : BaseSQLDataManager
    {
        public SourceCustomTimeZoneInfoDataManager(string connectionString)
            : base(connectionString, false)
        {
        }

        public List<SourceCustomTimeZoneInfo> GetSourceCustomTimeZonesInfo()
        {
            return GetItemsText(query_getSourceCustomTimeZonesInfo, SourceCustomTimeZoneInfoMapper, null);
        }

        private SourceCustomTimeZoneInfo SourceCustomTimeZoneInfoMapper(IDataReader reader)
        {
            SourceCustomTimeZoneInfo sourceCustomTimeZoneInfo = new SourceCustomTimeZoneInfo()
            {
                SourceId = reader["ID"].ToString(),
                BaseUtcOffset = (int)reader["BaseUtcOffset"],
                DisplayName = reader["DisplayName"] as string
            };
            return sourceCustomTimeZoneInfo;
        }

        const string query_getSourceCustomTimeZonesInfo = @"SELECT [ID], [BaseUtcOffset], [DisplayName]  FROM [dbo].[CustomTimeZoneInfo] WITH (NOLOCK)";
    }
}
