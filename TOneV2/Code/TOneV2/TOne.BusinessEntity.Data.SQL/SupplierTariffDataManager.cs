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
            Dictionary<string, string> mapper = new Dictionary<string, string>();

            mapper.Add("SupplierName", "SupplierID");
            mapper.Add("ZoneName", "ZoneID");
            mapper.Add("EndEffectiveDateDescription", "EndEffectiveDate");

            return RetrieveData(input, (tempTableName) =>
            {
                string zoneIDs = null;

                if (input.Query.selectedZoneIDs != null && input.Query.selectedZoneIDs.Count() > 0)
                    zoneIDs = string.Join<int>(",", input.Query.selectedZoneIDs);

                ExecuteNonQuerySP("BEntity.sp_Tariff_GetFilteredBySupplierID", tempTableName, input.Query.selectedSupplierID, zoneIDs, input.Query.effectiveOn);

            }, (reader) => SupplierTariffMapper(reader));
        }

        private SupplierTariff SupplierTariffMapper(IDataReader reader)
        {
            SupplierTariff supplierTariff = new SupplierTariff
            {
                TariffID = (long)reader["TariffID"],
                SupplierID = reader["SupplierID"] as string,
                SupplierName = reader["SupplierName"] as string,
                ZoneID = GetReaderValue<int>(reader, "ZoneID"),
                ZoneName = reader["ZoneName"] as string,
                CurrencyID = GetReaderValue<string>(reader, "CurrencyID"),
                CallFee = (decimal)reader["CallFee"],
                FirstPeriod = GetReaderValue<byte>(reader, "FirstPeriod"),
                FirstPeriodRate = GetReaderValue<decimal>(reader, "FirstPeriodRate"),
                FractionUnit = GetReaderValue<byte>(reader, "FractionUnit"),
                BeginEffectiveDate = GetReaderValue<DateTime>(reader, "BeginEffectiveDate"),
                EndEffectiveDate = GetReaderValue<DateTime>(reader, "EndEffectiveDate"),
                EndEffectiveDateDescription = GetReaderValue<string>(reader, "EndEffectiveDate"),
                IsEffective = (string)reader["IsEffective"]
            };

            return supplierTariff;
        }
    }
}
