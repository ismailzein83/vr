﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SourceZoneServiceDataManager : BaseSQLDataManager
    {
        DateTime? _effectiveFrom;
        bool _onlyEffective;
        public SourceZoneServiceDataManager(string connectionString, DateTime? effectiveFrom, bool onlyEffective)
            : base(connectionString, false)
        {
            _effectiveFrom = effectiveFrom;
            _onlyEffective = onlyEffective;
        }

        public List<SourceRate> GetSourceZoneServices(bool isSaleZoneService)
        {

            return GetItemsText((isSaleZoneService ? query_getSourceZonesServices_Sale : query_getSourceZonesServices_Purchase) + MigrationUtils.GetEffectiveQuery("Rate", _onlyEffective, _effectiveFrom), SourceRateMapper, null);
        }


        public void LoadSourceItems(bool isSaleZoneService, Action<SourceRate> itemToAdd)
        {
            ExecuteReaderText((isSaleZoneService ? query_getSourceZonesServices_Sale : query_getSourceZonesServices_Purchase) + MigrationUtils.GetEffectiveQuery("Rate", _onlyEffective, _effectiveFrom), (reader) =>
            {
                while (reader.Read())
                    itemToAdd(SourceRateMapper(reader));

            }, null);
        }


        private SourceRate SourceRateMapper(IDataReader reader)
        {
            return new SourceRate()
            {
                SourceId = reader["RateID"].ToString(),
                ZoneId = GetReaderValue<int?>(reader, "ZoneID"),
                PriceListId = GetReaderValue<int?>(reader, "PriceListID"),
                BeginEffectiveDate = GetReaderValue<DateTime?>(reader, "BeginEffectiveDate"),
                EndEffectiveDate = GetReaderValue<DateTime?>(reader, "EndEffectiveDate"),
                ServicesFlag = GetReaderValue<Int16>(reader, "ServicesFlag")
            };
        }


        const string query_getSourceZonesServices_Sale = @"SELECT    Rate.RateID RateID, Rate.PriceListID PriceListID, Rate.ZoneID ZoneID,
                                                        Rate.Rate Rate, Rate.OffPeakRate OffPeakRate, Rate.WeekendRate WeekendRate,
                                                        Rate.BeginEffectiveDate BeginEffectiveDate, Rate.EndEffectiveDate EndEffectiveDate,
                                                        Rate.Change, Rate.ServicesFlag, PriceList.CurrencyID CurrencyID , Rate.ServicesFlag
                                                        FROM Rate WITH (NOLOCK) INNER JOIN
                                                        Zone WITH (NOLOCK) ON Rate.ZoneID = Zone.ZoneID INNER JOIN
                                                        PriceList WITH (NOLOCK) ON Rate.PriceListID = PriceList.PriceListID 
                                                        Inner Join CarrierAccount ca on ca.CarrierAccountID = PriceList.CustomerID
														where Zone.SupplierID = 'SYS'  and ca.AccountType <> 2  and Zone.CodeGroup <> '-' ";

        const string query_getSourceZonesServices_Purchase = @"SELECT    Rate.RateID RateID, Rate.PriceListID PriceListID, Rate.ZoneID ZoneID,
                                                        Rate.Rate Rate, Rate.OffPeakRate OffPeakRate, Rate.WeekendRate WeekendRate,
                                                        Rate.BeginEffectiveDate BeginEffectiveDate, Rate.EndEffectiveDate EndEffectiveDate,
                                                        Rate.Change, Rate.ServicesFlag, PriceList.CurrencyID CurrencyID , Rate.ServicesFlag
                                                        FROM Rate WITH (NOLOCK) INNER JOIN
                                                        Zone WITH (NOLOCK) ON Rate.ZoneID = Zone.ZoneID INNER JOIN
                                                        PriceList WITH (NOLOCK) ON Rate.PriceListID = PriceList.PriceListID 
														Inner Join CarrierAccount ca on ca.CarrierAccountID = PriceList.SupplierID
														where Zone.SupplierID <> 'SYS'  and ca.AccountType <> 0  and Zone.CodeGroup <> '-' ";

    }
}
