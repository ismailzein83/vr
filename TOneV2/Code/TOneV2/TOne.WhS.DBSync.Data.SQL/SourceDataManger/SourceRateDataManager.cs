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
        DateTime? _effectiveFrom;
        bool _onlyEffective;
        public SourceRateDataManager(string connectionString, DateTime? effectiveFrom, bool onlyEffective)
            : base(connectionString, false)
        {
            _onlyEffective = onlyEffective;
            _effectiveFrom = effectiveFrom;
        }

        public List<SourceRate> GetSourceRates(bool isSaleRate)
        {
            return GetItemsText((isSaleRate ? query_getSourceRates_Sale : query_getSourceRates_Purchase) + MigrationUtils.GetEffectiveQuery("Rate", _onlyEffective, _effectiveFrom), SourceRateMapper, null);
        }


        public void LoadSourceItems(bool isSaleRate, Action<SourceRate> itemToAdd)
        {
            ExecuteReaderText((isSaleRate ? query_getSourceRates_Sale : query_getSourceRates_Purchase) + MigrationUtils.GetEffectiveQuery("Rate", _onlyEffective, _effectiveFrom), (reader) =>
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


        const string query_getSourceRates_Sale = @"SELECT Rate.RateID RateID, Rate.PriceListID PriceListID, Rate.ZoneID ZoneID,
                                                        Rate.Rate Rate, Rate.OffPeakRate OffPeakRate, Rate.WeekendRate WeekendRate,
                                                        Rate.BeginEffectiveDate BeginEffectiveDate, Rate.EndEffectiveDate EndEffectiveDate,
                                                        Rate.Change, Rate.ServicesFlag, PriceList.CurrencyID CurrencyID, PriceList.CustomerID
                                                        FROM Rate WITH (NOLOCK) 
														INNER JOIN Zone WITH (NOLOCK) ON Rate.ZoneID = Zone.ZoneID 
														INNER JOIN PriceList WITH (NOLOCK) ON Rate.PriceListID = PriceList.PriceListID
														Inner Join CarrierAccount ca on ca.CarrierAccountID = PriceList.CustomerID
														where Zone.SupplierID = 'SYS'  and ca.AccountType <> 2  and Zone.CodeGroup <> '-'";

        const string query_getSourceRates_Purchase = @"SELECT Rate.RateID RateID, Rate.PriceListID PriceListID, Rate.ZoneID ZoneID,
                                                        Rate.Rate Rate, Rate.OffPeakRate OffPeakRate, Rate.WeekendRate WeekendRate,
                                                        Rate.BeginEffectiveDate BeginEffectiveDate, Rate.EndEffectiveDate EndEffectiveDate,
                                                        Rate.Change, Rate.ServicesFlag, PriceList.CurrencyID CurrencyID, PriceList.CustomerID
                                                        FROM Rate WITH (NOLOCK) 
														INNER JOIN Zone WITH (NOLOCK) ON Rate.ZoneID = Zone.ZoneID 
														INNER JOIN PriceList WITH (NOLOCK) ON Rate.PriceListID = PriceList.PriceListID
														Inner Join CarrierAccount ca on ca.CarrierAccountID = PriceList.SupplierID
														where Zone.SupplierID <> 'SYS'  and ca.AccountType <> 0  and Zone.CodeGroup <> '-'";
    }
}
