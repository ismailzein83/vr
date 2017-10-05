﻿using System;
using System.Collections.Generic;
using System.Data;
using TOne.WhS.BusinessEntity.Entities;
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

        public List<SourceRate> GetSourceRates(bool isSaleRate, bool onlyEffective)
        {
            string queryOnlyEffective = onlyEffective ? "and Rate.IsEffective = 'Y'" : string.Empty;
            return GetItemsText(query_getSourceRates + (isSaleRate ? "where Zone.SupplierID = 'SYS'" : "where Zone.SupplierID <> 'SYS'") + queryOnlyEffective, SourceRateMapper, null);
        }


        public void LoadSourceItems(bool isSaleRate, bool onlyEffective, Action<SourceRate> itemToAdd)
        {
            string queryOnlyEffective = onlyEffective ? "and Rate.IsEffective = 'Y'" : string.Empty;
            ExecuteReaderText(query_getSourceRates + (isSaleRate ? "where Zone.SupplierID = 'SYS'" : "where Zone.SupplierID <> 'SYS'") + queryOnlyEffective, (reader) =>
            {
                while (reader.Read())
                {
                    decimal? offPeakRate = GetReaderValue<decimal?>(reader, "OffPeakRate");
                    decimal? weekendRate = GetReaderValue<decimal?>(reader, "WeekendRate");
                    decimal? normalRate = GetReaderValue<decimal?>(reader, "Rate");

                    if (offPeakRate.HasValue)
                        itemToAdd(SourceOtherRateMapper(reader, offPeakRate, RateTypeEnum.OffPeak));

                    if (weekendRate.HasValue)
                        itemToAdd(SourceOtherRateMapper(reader, weekendRate, RateTypeEnum.Weekend));

                    itemToAdd(SourceRateMapper(reader));
                }
            }, null);
        }

        private SourceRate SourceRateMapper(IDataReader reader)
        {
            return new SourceRate()
            {
                SourceId = reader["RateID"].ToString(),
                PriceListId = GetReaderValue<int?>(reader, "PriceListID"),
                ZoneId = GetReaderValue<int?>(reader, "ZoneID"),
                Rate = GetReaderValue<decimal?>(reader, "Rate"),
                BeginEffectiveDate = GetReaderValue<DateTime?>(reader, "BeginEffectiveDate"),
                EndEffectiveDate = GetReaderValue<DateTime?>(reader, "EndEffectiveDate"),
                Change = GetReaderValue<Int16>(reader, "Change"),
                ServicesFlag = GetReaderValue<Int16>(reader, "ServicesFlag"),
                CurrencyId = reader["CurrencyID"] as string,
                CustomerId = reader["CustomerID"] as string
            };
        }

        private SourceRate SourceOtherRateMapper(IDataReader reader, decimal? rate, RateTypeEnum rateType)
        {
            return new SourceRate()
            {
                SourceId = reader["RateID"].ToString(),
                PriceListId = GetReaderValue<int?>(reader, "PriceListID"),
                ZoneId = GetReaderValue<int?>(reader, "ZoneID"),
                Rate = rate.Value,
                BeginEffectiveDate = GetReaderValue<DateTime?>(reader, "BeginEffectiveDate"),
                EndEffectiveDate = GetReaderValue<DateTime?>(reader, "EndEffectiveDate"),
                Change = GetReaderValue<Int16>(reader, "Change"),
                ServicesFlag = GetReaderValue<Int16>(reader, "ServicesFlag"),
                CurrencyId = reader["CurrencyID"] as string,
                RateType = rateType,
                CustomerId = reader["CustomerID"] as string
            };
        }


        const string query_getSourceRates = @"SELECT    Rate.RateID RateID, Rate.PriceListID PriceListID, Rate.ZoneID ZoneID,
                                                        Rate.Rate Rate, Rate.OffPeakRate OffPeakRate, Rate.WeekendRate WeekendRate,
                                                        Rate.BeginEffectiveDate BeginEffectiveDate, Rate.EndEffectiveDate EndEffectiveDate,
                                                        Rate.Change, Rate.ServicesFlag, PriceList.CurrencyID CurrencyID, PriceList.CustomerID
                                                        FROM Rate WITH (NOLOCK) INNER JOIN
                                                        Zone WITH (NOLOCK) ON Rate.ZoneID = Zone.ZoneID INNER JOIN
                                                        PriceList WITH (NOLOCK) ON Rate.PriceListID = PriceList.PriceListID ";
    }
}
