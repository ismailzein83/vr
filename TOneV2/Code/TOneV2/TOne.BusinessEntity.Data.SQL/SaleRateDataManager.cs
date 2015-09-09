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
    public class SaleRateDataManager : BaseTOneDataManager, ISaleRateDataManager
    {
        public List<Rate> GetSaleRates(string customerID, DateTime when)
        {
            return GetItemsSP("BEntity.sp_Rate_GetSaleByCustomer", RateMapper, customerID, when);
        }
        private Rate RateMapper(IDataReader reader)
        {
            return new Rate
            {
                RateId = (long)reader["RateID"],
                SupplierId = reader["SupplierID"] as string,
                CustomerId = reader["CustomerID"] as string,
                ZoneId = GetReaderValue<int>(reader, "ZoneId"),
                NormalRate = GetReaderValue<decimal>(reader, "Rate"),
                OffPeakRate = GetReaderValue<decimal?>(reader, "OffPeakRate"),
                WeekendRate = GetReaderValue<decimal?>(reader, "WeekendRate"),
                PriceListId = GetReaderValue<int>(reader, "PricelistId"),
                ServicesFlag = GetReaderValue<short>(reader, "ServicesFlag"),
                BeginEffectiveDate = GetReaderValue<DateTime?>(reader, "RateBeginEffectiveDate"),
                EndEffectiveDate = GetReaderValue<DateTime?>(reader, "RateEndEffectiveDate"),
                CurrencyID = reader["CurrencyID"] as string,
                CurrencyLastRate = (float)GetReaderValue<double>(reader, "CurrencyLastRate"),
                Change = TOne.BusinessEntity.Entities.Change.None
            };
        }

    }
}
