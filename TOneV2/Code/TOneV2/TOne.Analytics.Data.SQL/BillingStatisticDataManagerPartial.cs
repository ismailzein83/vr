using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;
using TOne.Data.SQL;

namespace TOne.Analytics.Data.SQL
{
    partial class BillingStatisticDataManager : BaseTOneDataManager, IBillingStatisticDataManager
    {
        public List<CustomerSummary> GetCustomerSummary(DateTime fromDate, DateTime toDate, string customerId, int? customerAMUId, int? supplierAMUId)
        {
            return GetItemsSP("Analytics.SP_Billing_CustomerSummary", CustomerSummaryMapper,
               (customerId == null || customerId == "") ? null : customerId,
               fromDate,
               toDate,
               (customerAMUId == 0 || customerAMUId == null) ? (object)DBNull.Value : customerAMUId,
               (supplierAMUId == 0 || supplierAMUId == null) ? (object)DBNull.Value : supplierAMUId
               );
        }
        public List<CustomerServices> GetCustomerServices(DateTime fromDate, DateTime toDate)
        {
            return GetItemsSP("Analytics.SP_Billing_GetCustomerSummary",  CustomerServicesMapper,
              fromDate,
              toDate
            );
        }

        #region privateMethods
        private CustomerSummary CustomerSummaryMapper(IDataReader reader)
        {
            CustomerSummary instance = new CustomerSummary
            {
                Carrier = reader["Carrier"] as string,
                SaleDuration = GetReaderValue<decimal>(reader, "SaleDuration"),
                SaleNet = GetReaderValue<double>(reader, "SaleNet"),
                CostDuration = GetReaderValue<decimal>(reader, "CostDuration"),
                CostNet = GetReaderValue<double>(reader, "CostNet")
            };
            return instance;
        }

        private CustomerServices CustomerServicesMapper(IDataReader reader)
        {
            CustomerServices instance = new CustomerServices
            {
                AccountId = reader["AccountID"] as string ,
                Services = GetReaderValue<decimal>(reader, "Services")
            };
            return instance;
        }
        #endregion
    }
}
