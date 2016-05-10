using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.Analytics.Entities.BillingReport;

namespace TOne.WhS.Analytics.Data.SQL
{
    partial class BillingStatisticDataManager : BaseTOneDataManager, IBillingStatisticDataManager
    {
        #region ctor/Local Variables
        public BillingStatisticDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
        }
        #endregion

        public List<ZoneProfit> GetZoneProfit(DateTime fromDate, DateTime toDate, string customerIds, string supplierIds, int currencyId)
        {
            return GetItemsSP("[TOneWhS_Billing].[SP_BillingRep_GetZoneProfitsV2]", (reader) => ZoneProfitMapper(reader),
                fromDate,
                toDate,
                customerIds,
                supplierIds,
                currencyId
                );
        }

        #region PrivatMethods
        private ZoneProfit ZoneProfitMapper(IDataReader reader)
        {
            ZoneProfit instance = new ZoneProfit
            {
                SaleZoneID =  GetReaderValue<long>(reader, "SaleZoneID"),
                SupplierZoneID = GetReaderValue<long>(reader, "SupplierZoneID"),
                SupplierID = GetReaderValue<int>(reader, "SupplierID"),
                Calls = GetReaderValue<int>(reader, "Calls"),
                SaleDuration = GetReaderValue<decimal>(reader, "SaleDuration"),
                CostDuration = GetReaderValue<decimal>(reader, "CostDuration"),
                DurationNet = GetReaderValue<decimal>(reader, "DurationNet"),
                SaleNet = GetReaderValue<double>(reader, "SaleNet"),
                CostNet = GetReaderValue<double>(reader, "CostNet"),
            };

            return instance;
        }
        #endregion
    }
}
