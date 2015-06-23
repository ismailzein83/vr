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
            return GetItemsSP("Analytics.SP_Billing_CustomerServices", CustomerServicesMapper,
              fromDate,
              toDate
            );
        }

        public List<CustomerRouting> GetCustomerRouting(DateTime fromDate, DateTime toDate, string customerId, string supplierId, int? customerAMUId, int? supplierAMUId)
        {
            return GetItemsSP("Analytics.SP_Billing_CustomerRouting", CustomerRoutingMapper,               
               fromDate,
               toDate,
               (customerId == null || customerId == "") ? null : customerId,
               (supplierId == null || supplierId == "") ? null : supplierId,
               (customerAMUId == 0 || customerAMUId == null) ? (object)DBNull.Value : customerAMUId,
               (supplierAMUId == 0 || supplierAMUId == null) ? (object)DBNull.Value : supplierAMUId
               );
        }

        public List<RoutingAnalysis> GetRoutingAnalysis(DateTime fromDate, DateTime toDate, string customerId, string supplierId, int? top, int? customerAMUId, int? supplierAMUId)
        {
            return GetItemsSP("Analytics.SP_Billing_RoutingAnalysis", RoutingAnalysisMapper,
               fromDate,
               toDate,
               (customerId == null || customerId == "") ? null : customerId,
               (supplierId == null || supplierId == "") ? null : supplierId,
               (top == null || top == 0) ? null : top,
               (customerAMUId == 0 || customerAMUId == null) ? (object)DBNull.Value : customerAMUId,
               (supplierAMUId == 0 || supplierAMUId == null) ? (object)DBNull.Value : supplierAMUId
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
                AccountId = reader["AccountID"] as string,
                Services = GetReaderValue<double>(reader, "Services")
            };
            return instance;
        }

        private CustomerRouting CustomerRoutingMapper(IDataReader reader)
        {
            CustomerRouting instance = new CustomerRouting
            {
                CallDate = GetReaderValue<DateTime>(reader,"CallDate"),
                SupplierID = reader["SupplierID"] as string,
                CustomerID =reader["CustomerID"] as string ,
                SaleZone = GetReaderValue<int>(reader, "SaleZone"),
                SaleDuration = GetReaderValue<decimal>(reader, "SaleDuration"),
                SaleRate = GetReaderValue<double>(reader, "SaleRate"),
                SaleNet = GetReaderValue<double>(reader, "SaleNet"),
                CostZone = GetReaderValue<int>(reader, "CostZone"),
                CostDuration = GetReaderValue<decimal>(reader, "CostDuration"),
                CostNet = GetReaderValue<double>(reader, "CostNet"),
                CostRate = GetReaderValue<double>(reader, "CostRate")
            };
            return instance;
        }

        private RoutingAnalysis RoutingAnalysisMapper(IDataReader reader)
        {
            RoutingAnalysis instance = new RoutingAnalysis
            {
                
                SupplierID = reader["SupplierID"] as string,
                SaleNet = GetReaderValue<double>(reader, "SaleNet"),               
                CostNet = GetReaderValue<double>(reader, "CostNet"),
                Duration = GetReaderValue<decimal>(reader, "Duration"),
                SaleZoneID = GetReaderValue<int>(reader, "SaleZoneID"),
                ACD = GetReaderValue<decimal>(reader, "ACD"),
                ASR = GetReaderValue<decimal>(reader, "ASR")
            };
            return instance;
        }
        #endregion
    }
}
