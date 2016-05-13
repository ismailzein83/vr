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

        public List<ZoneSummary> GetZoneSummary(DateTime fromDate, DateTime toDate, string customerId, string supplierId, bool isCost, int currencyId, string supplierGroup, string customerGroup, List<string> customerIds, List<string> supplierIds, bool groupBySupplier, out double services)
        {
            List<ZoneSummary> lstZoneSummary = new List<ZoneSummary>();
            double servicesFees = 0;
            string suppliersIds = null;
            if (supplierIds != null && supplierIds.Count > 0)
                suppliersIds = string.Join<string>(",", supplierIds);
            string customersIds = null;
            if (customerIds != null && customerIds.Count > 0)
                customersIds = string.Join<string>(",", customerIds);

            ExecuteReaderSP("[TOneWhS_Billing].[SP_BillingRep_GetZoneSummary]", (reader) =>
            {
                while (reader.Read())
                {
                    lstZoneSummary.Add(new ZoneSummary
                    {
                        Zone = reader["Zone"] as string,
                        Calls = GetReaderValue<int>(reader, "Calls"),
                        Rate = GetReaderValue<double>(reader, "Rate"),
                        DurationNet = GetReaderValue<decimal>(reader, "DurationNet"),
                        DurationInSeconds = GetReaderValue<decimal>(reader, "DurationInSeconds"),
                        RateType = GetReaderValue<byte>(reader, "RateType"),
                        Net = GetReaderValue<double>(reader, "Net"),
                        CommissionValue = GetReaderValue<double>(reader, "CommissionValue"),
                        ExtraChargeValue = GetReaderValue<double>(reader, "ExtraChargeValue"),
                        SupplierID = (groupBySupplier ? GetReaderValue<int?>(reader, "SupplierID") : null)
                    });
                }

                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        servicesFees = GetReaderValue<double>(reader, "Services");
                    }
                }
            },
               fromDate,
               toDate,
               (customerId == null || customerId == "") ? null : customerId,
               (supplierId == null || supplierId == "") ? null : supplierId,
               isCost,
               currencyId,
               (supplierGroup == null || supplierGroup == "") ? null : supplierGroup,
               (customerGroup == null || customerGroup == "") ? null : customerGroup,
               customersIds,
               suppliersIds,
               groupBySupplier);
            services = servicesFees;
            return lstZoneSummary;
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
