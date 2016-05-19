using System;
using System.Collections.Generic;
using System.Data;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SourceRateDataManager : BaseSQLDataManager
    {
        public SourceRateDataManager(string connectionString)
            : base(connectionString, false)
        {
        }

        public List<SourceRate> GetSourceRates()
        {
            return GetItemsText(query_getSourceRates, SourceRateMapper, null);
        }

        private SourceRate SourceRateMapper(IDataReader arg)
        {
            return new SourceRate()
            {
                SourceId = arg["RateID"].ToString(),
                PriceListId = GetReaderValue<int?>(arg, "PriceListID"),
                ZoneId = GetReaderValue<int?>(arg, "ZoneID"),
                Rate = GetReaderValue<int?>(arg, "Rate"),
                OffPeakRate = GetReaderValue<int?>(arg, "OffPeakRate"),
                WeekendRate = GetReaderValue<int?>(arg, "WeekendRate"),
                BeginEffectiveDate = GetReaderValue<DateTime?>(arg, "BeginEffectiveDate"),
                EndEffectiveDate = GetReaderValue<DateTime?>(arg, "EndEffectiveDate"),
                Notes = arg["Notes"] as string,
            };
        }

        const string query_getSourceRates = @"SELECT [RateID] ,[PriceListID]  ,[ZoneID]  ,[Rate]  ,[OffPeakRate]  ,[WeekendRate]  ,[BeginEffectiveDate]  ,[EndEffectiveDate]  ,[Notes]  FROM [dbo].[Rate] WITH (NOLOCK)";
    }
}
