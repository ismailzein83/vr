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

        public List<SupplierCostDetails> GetSupplierCostDetails(DateTime fromDate, DateTime toDate, int? customerAMUId, int? supplierAMUId)
        {
            return GetItemsSP("Analytics.SP_Billing_SupplierCostDetails", SupplierCostDetailsMapper,
               fromDate,
               toDate,
               (supplierAMUId == 0 || supplierAMUId == null) ? (object)DBNull.Value : supplierAMUId,
               (customerAMUId == 0 || customerAMUId == null) ? (object)DBNull.Value : customerAMUId
               );
        }

        public List<SaleZoneCostSummary> GetSaleZoneCostSummary(DateTime fromDate, DateTime toDate, int? customerAMUId, int? supplierAMUId)
        {
            return GetItemsSP("Analytics.SP_Billing_SaleZoneCostSummary", SaleZoneCostSummaryMapper,
               fromDate,
               toDate,
               "AverageCost",
               (supplierAMUId == 0 || supplierAMUId == null) ? (object)DBNull.Value : supplierAMUId,
               (customerAMUId == 0 || customerAMUId == null) ? (object)DBNull.Value : customerAMUId
               );
        }

        public List<SaleZoneCostSummaryService> GetSaleZoneCostSummaryService(DateTime fromDate, DateTime toDate, int? customerAMUId, int? supplierAMUId)
        {
            return GetItemsSP("Analytics.SP_Billing_SaleZoneCostSummary", SaleZoneCostSummaryServiceMapper,
               fromDate,
               toDate,
               "Service",
               (supplierAMUId == 0 || supplierAMUId == null) ? (object)DBNull.Value : supplierAMUId,
               (customerAMUId == 0 || customerAMUId == null) ? (object)DBNull.Value : customerAMUId
               );
        }

        public List<SaleZoneCostSummarySupplier> GetSaleZoneCostSummarySupplier(DateTime fromDate, DateTime toDate, int? customerAMUId, int? supplierAMUId)
        {
            return GetItemsSP("Analytics.SP_Billing_SaleZoneCostSummary", SaleZoneCostSummarySupplierMapper,
               fromDate,
               toDate,
               "Supplier",
               (supplierAMUId == 0 || supplierAMUId == null) ? (object)DBNull.Value : supplierAMUId,
               (customerAMUId == 0 || customerAMUId == null) ? (object)DBNull.Value : customerAMUId
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

        private SupplierCostDetails SupplierCostDetailsMapper(IDataReader reader)
        {
            SupplierCostDetails instance = new SupplierCostDetails
            {
                Customer = reader["Customer"] as string,
                Carrier = reader["Carrier"] as string,
                Duration = GetReaderValue<decimal>(reader, "Duration"),
                Amount = GetReaderValue<double>(reader, "Amount")
            };
            return instance;
        }

        private SaleZoneCostSummary SaleZoneCostSummaryMapper(IDataReader reader)
        {
            SaleZoneCostSummary instance = new SaleZoneCostSummary
            {
                AvgCost = GetReaderValue<double>(reader, "AvgCost"),
                salezoneID = GetReaderValue<int>(reader, "salezoneID"),
                AvgDuration = GetReaderValue<decimal>(reader, "AvgDuration")
            };
            return instance;
        }

        private SaleZoneCostSummaryService SaleZoneCostSummaryServiceMapper(IDataReader reader)
        {
            SaleZoneCostSummaryService instance = new SaleZoneCostSummaryService
            {
                AvgServiceCost = GetReaderValue<double>(reader, "AvgServiceCost"),
                salezoneID = GetReaderValue<int>(reader, "salezoneID"),
                Service = reader["Service"] as string,
                AvgDuration = GetReaderValue<decimal>(reader, "AvgDuration")
            };
            return instance;
        }

        private SaleZoneCostSummarySupplier SaleZoneCostSummarySupplierMapper(IDataReader reader)
        {
            SaleZoneCostSummarySupplier instance = new SaleZoneCostSummarySupplier
            {
                SupplierID = reader["SupplierID"] as string,
                HighestRate = GetReaderValue<double>(reader, "HighestRate"),
                salezoneID = GetReaderValue<int>(reader, "salezoneID"),
                AvgDuration = GetReaderValue<decimal>(reader, "AvgDuration")
            };
            return instance;
        }
        #endregion
    }
}
