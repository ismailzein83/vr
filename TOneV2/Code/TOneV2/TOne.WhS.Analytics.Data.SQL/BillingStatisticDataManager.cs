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

        public List<ZoneProfit> GetZoneProfit(DateTime fromDate, DateTime toDate, string currencyId)
        {
            return GetItemsSP("[TOneWhS_Billing].[SP_BillingRep_GetZoneProfitsV2]", (reader) => ZoneProfitMapper(reader),
                fromDate,
                toDate,
                currencyId
                );
        }

        public List<ZoneProfit> GetZoneProfitOld(DateTime fromDate, DateTime toDate, int customerId, int supplierId, bool groupByCustomer, List<string> supplierIds, List<string> customerIds, string currencyId)
        {
            string suppliersIds = null;
            if (supplierIds != null && supplierIds.Count() > 0)
                suppliersIds = string.Join<string>(",", supplierIds);
            string customersIds = null;
            if (customerIds != null && customerIds.Count() > 0)
                customersIds = string.Join<string>(",", customerIds);

            return GetItemsSP("[TOneWhS_Billing].[SP_BillingRep_GetZoneProfits]", (reader) => ZoneProfitMapper(reader, groupByCustomer),
                fromDate,
                toDate,
                null,
                null,
                groupByCustomer,
                null,
                null,
                currencyId
                );
            //return GetItemsSP("[TOneWhS_Billing].[SP_BillingRep_GetZoneProfits]", (reader) => ZoneProfitMapper(reader, groupByCustomer),
            //    fromDate,
            //    toDate,
            //    customerId,
            //    supplierId,
            //    groupByCustomer,
            //    customersIds,
            //    suppliersIds,
            //    currencyId
            //    );
        }
        #region PrivatMethods
        private ZoneProfit ZoneProfitMapper(IDataReader reader, bool groupByCustomer)
        {
            ZoneProfit instance = new ZoneProfit
            {
                //CostZone = reader["CostZone"] as string,
                //SaleZone = reader["SaleZone"] as string,
                SupplierID = GetReaderValue<int>(reader, "SupplierID"),
                Calls = GetReaderValue<int>(reader, "Calls"),
                SaleDuration = GetReaderValue<decimal>(reader, "SaleDuration"),
                SaleNet = GetReaderValue<double>(reader, "SaleNet"),
                CostNet = GetReaderValue<double>(reader, "CostNet"),
                CostDuration = GetReaderValue<decimal>(reader, "CostDuration"),
                DurationNet = GetReaderValue<decimal>(reader, "DurationNet")
            };
            if (groupByCustomer)
                instance.CustomerId = GetReaderValue<int>(reader, "CustomerID");

            return instance;
        }
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
