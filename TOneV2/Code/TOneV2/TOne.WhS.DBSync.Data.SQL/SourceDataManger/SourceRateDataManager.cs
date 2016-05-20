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

        public List<SourceRate> GetSourceRates(bool isSaleRate)
        {
            return GetItemsText(query_getSourceRates + (isSaleRate ? "where Zone.SupplierID = 'SYS'" : ""), SourceRateMapper, null);
        }

        private SourceRate SourceRateMapper(IDataReader arg)
        {
            return new SourceRate()
            {
                SourceId = arg["RateID"].ToString(),
                PriceListId = GetReaderValue<int?>(arg, "PriceListID"),
                ZoneId = GetReaderValue<int?>(arg, "ZoneID"),
                Rate = GetReaderValue<decimal?>(arg, "Rate"),
                OffPeakRate = GetReaderValue<decimal?>(arg, "OffPeakRate"),
                WeekendRate = GetReaderValue<decimal?>(arg, "WeekendRate"),
                BeginEffectiveDate = GetReaderValue<DateTime?>(arg, "BeginEffectiveDate"),
                EndEffectiveDate = GetReaderValue<DateTime?>(arg, "EndEffectiveDate"),
                Notes = arg["Notes"] as string,
                CurrencyId = arg["CurrencyID"] as string,
            };
        }

        const string query_getSourceRates = @"SELECT    Rate.RateID RateID, Rate.PriceListID PriceListID, Rate.ZoneID ZoneID,
                                                        Rate.Rate Rate, Rate.OffPeakRate OffPeakRate, Rate.WeekendRate WeekendRate,
                                                        Rate.BeginEffectiveDate BeginEffectiveDate, Rate.EndEffectiveDate EndEffectiveDate,
                                                        Rate.Notes Notes, PriceList.CurrencyID CurrencyID
                                                        FROM Rate WITH (NOLOCK) INNER JOIN
                                                        Zone WITH (NOLOCK) ON Rate.ZoneID = Zone.ZoneID INNER JOIN
                                                        PriceList WITH (NOLOCK) ON Rate.PriceListID = PriceList.PriceListID ";
    }
}
