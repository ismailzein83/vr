using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;
using TOne.Data.SQL;

namespace TOne.BusinessEntity.Data.SQL
{
    public class CustomerTariffDataManger : BaseTOneDataManager, ICustomerTariffDataManager
    {
        public Vanrise.Entities.BigResult<CustomerTariff> GetFilteredCustomerTariffs(Vanrise.Entities.DataRetrievalInput<CustomerTariffQuery> input)
        {
            return RetrieveData(input, (tempTableName) =>
            {
                string zoneIDs = null;

                if (input.Query.SelectedZoneIDs != null && input.Query.SelectedZoneIDs.Count() > 0)
                    zoneIDs = string.Join<int>(",", input.Query.SelectedZoneIDs);

                ExecuteNonQuerySP("BEntity.sp_Tariff_CreateTempByCustomerID", tempTableName, input.Query.SelectedCustomerID, zoneIDs, input.Query.EffectiveOn);

            }, (reader) => CustomerTariffMapper(reader));
        }

        private CustomerTariff CustomerTariffMapper(IDataReader reader)
        {
            CustomerTariff customerTariff = new CustomerTariff
            {
                TariffID = (long)reader["TariffID"],
                CustomerID = reader["CustomerID"] as string,
                CustomerName = CarrierAccountDataManager.GetCarrierAccountName(reader["CustomerName"] as string, GetReaderValue<string>(reader, "CustomerNameSuffix")),
                ZoneID = GetReaderValue<int>(reader, "ZoneID"),
                ZoneName = reader["ZoneName"] as string,
                CurrencyID = GetReaderValue<string>(reader, "CurrencyID"),
                CallFee = (decimal)reader["CallFee"],
                FirstPeriod = GetReaderValue<byte>(reader, "FirstPeriod"),
                FirstPeriodRate = GetReaderValue<decimal>(reader, "FirstPeriodRate"),
                FractionUnit = GetReaderValue<byte>(reader, "FractionUnit"),
                BeginEffectiveDate = GetReaderValue<DateTime>(reader, "BeginEffectiveDate"),
                EndEffectiveDate = GetReaderValue<DateTime?>(reader, "EndEffectiveDate"),
                IsEffective = (string)reader["IsEffective"]
            };

            return customerTariff;
        }
    }
}
