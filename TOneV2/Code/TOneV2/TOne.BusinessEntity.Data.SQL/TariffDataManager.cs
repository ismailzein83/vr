using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;
using TOne.Data.SQL;

namespace TOne.BusinessEntity.Data.SQL
{
    public class TariffDataManager : BaseTOneDataManager, ITariffDataManager
    {
        private Tariff TariffMapper(IDataReader reader)
        {
            return new Tariff
            {
                TariffID = (int)reader["TariffID"],
                ZoneId = GetReaderValue<int>(reader, "ZoneID"),
                SupplierId = reader["SupplierID"] as string,
                CustomerId = reader["CustomerID"] as string,
                CallFee = GetReaderValue<decimal>(reader, "CallFee"),
                FirstPeriodRate = GetReaderValue<decimal>(reader, "FirstPeriodRate"),
                FirstPeriod = GetReaderValue<int>(reader, "FirstPeriod"),
                RepeatFirstPeriod = reader["RepeatFirstPeriod"] as string == "Y",
                FractionUnit = GetReaderValue<int>(reader, "FractionUnit"),
                BeginEffectiveDate = GetReaderValue<DateTime?>(reader, "BeginEffectiveDate"),
                EndEffectiveDate = GetReaderValue<DateTime?>(reader, "EndEffectiveDate")
            };
        }

        public List<Tariff> GetTariff(string customerId, int zoneId, DateTime when)
        {
            return GetItemsSP("BEntity.sp_Tariff_GetTariffs", TariffMapper, zoneId, customerId, when);
        }
    }
}
