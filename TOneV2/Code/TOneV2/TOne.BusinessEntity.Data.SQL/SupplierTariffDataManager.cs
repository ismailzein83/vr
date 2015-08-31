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
    public class SupplierTariffDataManager : BaseTOneDataManager, ISupplierTariffDataManager
    {
        public Vanrise.Entities.BigResult<SupplierTariff> GetFilteredSupplierTariffs(Vanrise.Entities.DataRetrievalInput<SupplierTariffQuery> input)
        {
            return RetrieveData(input, (tempTableName) =>
            {
                string zoneIDs = null;

                if (input.Query.SelectedZoneIDs != null && input.Query.SelectedZoneIDs.Count() > 0)
                    zoneIDs = string.Join<int>(",", input.Query.SelectedZoneIDs);

                ExecuteNonQuerySP("BEntity.sp_Tariff_CreateTempBySupplierID", tempTableName, input.Query.SelectedSupplierID, zoneIDs, input.Query.EffectiveOn);

            }, (reader) => SupplierTariffMapper(reader));
        }

        private SupplierTariff SupplierTariffMapper(IDataReader reader)
        {
            SupplierTariff supplierTariff = new SupplierTariff
            {
                TariffID = (long)reader["TariffID"],
                SupplierID = reader["SupplierID"] as string,
                SupplierName = CarrierAccountDataManager.GetCarrierAccountName(reader["SupplierName"] as string, GetReaderValue<string>(reader, "SupplierNameSuffix")),
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

            return supplierTariff;
        }
    }
}
