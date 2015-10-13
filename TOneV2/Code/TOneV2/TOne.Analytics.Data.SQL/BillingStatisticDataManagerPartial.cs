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
        public List<CustomerSummary> GetCustomerSummary(DateTime fromDate, DateTime toDate, string customerId,  List<string> customerIds, List<string> supplierIds, string currencyId)
        {
            string suppliersIds = null;
            if (supplierIds != null && supplierIds.Count() > 0)
                suppliersIds = string.Join<string>(",", supplierIds);
            string customersIds = null;
            if (customerIds != null && customerIds.Count() > 0)
                customersIds = string.Join<string>(",", customerIds);

            return GetItemsSP("Analytics.SP_BillingRep_GetCustomerSummary", CustomerSummaryMapper,
               (customerId == null || customerId == "") ? null : customerId,
               fromDate,
               toDate,
               customersIds,
               suppliersIds,
               currencyId
               );
        }
        public List<CustomerServices> GetCustomerServices(DateTime fromDate, DateTime toDate)
        {
            return GetItemsSP("Analytics.SP_BillingRep_GetCustomerServices", CustomerServicesMapper,
              fromDate,
              toDate
            );
        }

        public List<CustomerRouting> GetCustomerRouting(DateTime fromDate, DateTime toDate, string customerId, string supplierId, List<string> customerIds, List<string> supplierIds, string currencyId)
        {
            string suppliersIds = null;
            if (supplierIds != null && supplierIds.Count() > 0)
                suppliersIds = string.Join<string>(",", supplierIds);
            string customersIds = null;
            if (customerIds != null && customerIds.Count() > 0)
                customersIds = string.Join<string>(",", customerIds);
            return GetItemsSP("Analytics.SP_BillingRep_GetCustomerRouting", CustomerRoutingMapper,               
               fromDate,
               toDate,
               (customerId == null || customerId == "") ? null : customerId,
               (supplierId == null || supplierId == "") ? null : supplierId,
               customersIds,
               suppliersIds,
               currencyId
               );
        }

        public List<RoutingAnalysis> GetRoutingAnalysis(DateTime fromDate, DateTime toDate, string customerId, string supplierId, int? top, List<string> supplierIds, List<string> customerIds, string currencyId)
        {
            string suppliersIds = null;
            if (supplierIds != null && supplierIds.Count() > 0)
                suppliersIds = string.Join<string>(",", supplierIds);
            string customersIds = null;
            if (customerIds != null && customerIds.Count() > 0)
                customersIds = string.Join<string>(",", customerIds); 

            return GetItemsSP("Analytics.SP_BillingRep_GetRoutingAnalysis", RoutingAnalysisMapper,
               fromDate,
               toDate,
               (customerId == null || customerId == "") ? null : customerId,
               (supplierId == null || supplierId == "") ? null : supplierId,
               (top == null || top == 0) ? null : top,
               customersIds,
               suppliersIds,
               currencyId
               );
        }

        public List<SupplierCostDetails> GetSupplierCostDetails(DateTime fromDate, DateTime toDate, List<string> supplierIds, List<string> customerIds, string currencyId)
        {
            string suppliersIds = null;
            if (supplierIds != null && supplierIds.Count() > 0)
                suppliersIds = string.Join<string>(",", supplierIds);
            string customersIds = null;
            if (customerIds != null && customerIds.Count() > 0)
                customersIds = string.Join<string>(",", customerIds);

            return GetItemsSP("Analytics.SP_BillingRep_GetSupplierCostDetails", SupplierCostDetailsMapper,
               fromDate,
               toDate,
               customersIds,
               suppliersIds,
               currencyId
               );
        }

        public List<SaleZoneCostSummary> GetSaleZoneCostSummary(DateTime fromDate, DateTime toDate, List<string> customerIds, List<string> supplierIds, string CurrencyId)
        {
            string suppliersIds = null;
            if (supplierIds != null && supplierIds.Count() > 0)
                suppliersIds = string.Join<string>(",", supplierIds);
            string customersIds = null;
            if (customerIds != null && customerIds.Count() > 0)
                customersIds = string.Join<string>(",", customerIds);

            return GetItemsSP("Analytics.SP_BillingRep_GetSaleZoneCostSummary", SaleZoneCostSummaryMapper,
               fromDate,
               toDate,
               "AverageCost",
               customersIds,
               suppliersIds,
               CurrencyId
               );
        }

        public List<SaleZoneCostSummaryService> GetSaleZoneCostSummaryService(DateTime fromDate, DateTime toDate, List<string> customerIds, List<string> supplierIds, string CurrencyId)
        {
            string suppliersIds = null;
            if (supplierIds != null && supplierIds.Count() > 0)
                suppliersIds = string.Join<string>(",", supplierIds);
            string customersIds = null;
            if (customerIds != null && customerIds.Count() > 0)
                customersIds = string.Join<string>(",", customerIds);
            return GetItemsSP("Analytics.SP_BillingRep_GetSaleZoneCostSummary", SaleZoneCostSummaryServiceMapper,
               fromDate,
               toDate,
               "Service",
               customersIds,
               suppliersIds,
               CurrencyId
            );
        }

        public List<SaleZoneCostSummarySupplier> GetSaleZoneCostSummarySupplier(DateTime fromDate, DateTime toDate, List<string> customerIds, List<string> supplierIds, string CurrencyId)
        {
            string suppliersIds = null;
            if (supplierIds != null && supplierIds.Count() > 0)
                suppliersIds = string.Join<string>(",", supplierIds);
            string customersIds = null;
            if (customerIds != null && customerIds.Count() > 0)
                customersIds = string.Join<string>(",", customerIds);
            return GetItemsSP("Analytics.SP_BillingRep_GetSaleZoneCostSummary", SaleZoneCostSummarySupplierMapper,
               fromDate,
               toDate,
               "Supplier",
               customersIds,
               suppliersIds,
               CurrencyId
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
