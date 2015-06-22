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
            return GetItemsSP("Analytics.SP_Billing_CustomerSummary", CustomerRoutingMapper,               
               fromDate,
               toDate,
               (customerId == null || customerId == "") ? null : customerId,
               (supplierId == null || supplierId == "") ? null : supplierId,
               (customerAMUId == 0 || customerAMUId == null) ? (object)DBNull.Value : customerAMUId,
               (supplierAMUId == 0 || supplierAMUId == null) ? (object)DBNull.Value : supplierAMUId
               );
        }

       

        #region privateMethods

        private string GetVariationReportQuery(List<TimeRange> timeRange, VariationReportOptions variationReportOptions)
        {

            //  DateTime BeginTime = Convert.ToDateTime((timeRange.Compute("max(FromDate)", string.Empty)));
            //  DateTime EndTime = Convert.ToDateTime((timeRange.Compute("min(FromDate)", string.Empty)));

            DateTime BeginTime = (from d in timeRange select d.FromDate).Max();
            DateTime EndTime = (from d in timeRange select d.FromDate).Min();

            StringBuilder query = new StringBuilder(@"DECLARE @ExchangeRates TABLE(
		                                             Currency VARCHAR(3),
		                                             Date SMALLDATETIME,
		                                             Rate FLOAT
		                                             PRIMARY KEY(Currency, Date))
                                            INSERT INTO @ExchangeRates 
                                            SELECT * FROM dbo.GetDailyExchangeRates('@EndTime','@BeginTime')");
            query.Append(@" SELECT  #NameColumn# , 
            0.0 as [AVG],
            0.0 as [%], 
            0.0 as [Prev %],
            tr.FromDate,
            tr.ToDate,
            (#ValueColumn#) as Total,
            #IDColumn# as ID
            From @timeRange tr
            LEFT JOIN Billing_Stats BS ON BS.CallDate >= tr.FromDate AND BS.CallDate < tr.ToDate
            LEFT JOIN @ExchangeRates ERC ON ERC.Currency = BS.Cost_Currency AND ERC.Date = BS.CallDate			
            LEFT JOIN @ExchangeRates ERS ON ERS.Currency = BS.Sale_Currency AND ERS.Date = BS.CallDate        
            #JoinStatement#
            #WhereStatement#  
            GROUP BY #NameColumn# ,#IDColumn#,tr.FromDate, tr.ToDate
            ORDER BY tr.FromDate, tr.ToDate ");
            switch (variationReportOptions)
            {
                case VariationReportOptions.InBoundMinutes:
                    query.Replace("#NameColumn#", " cpc.Name ");
                    query.Replace("#ValueColumn#", " SUM(SaleDuration)/60 ");
                    query.Replace("#IDColumn#", " cac.CarrierAccountID ");
                    query.Replace("#JoinStatement#", @" JOIN CarrierAccount cac With(Nolock) ON cac.CarrierAccountID=BS.CustomerID
                                                        JOIN CarrierProfile cpc With(Nolock) ON cpc.ProfileID = cac.ProfileID ");
                    query.Replace("#WhereStatement#", " ");
                    break;

                case VariationReportOptions.OutBoundMinutes:
                    query.Replace("#NameColumn#", " cps.Name ");
                    query.Replace("#ValueColumn#", " SUM(CostDuration)/60 ");
                    query.Replace("#IDColumn#", " cas.CarrierAccountID ");
                    query.Replace("#JoinStatement#", @" JOIN CarrierAccount cas With(Nolock) ON cas.CarrierAccountID=BS.SupplierID
                                                        JOIN CarrierProfile cps With(Nolock) ON cps.ProfileID = cas.ProfileID ");
                    query.Replace("#WhereStatement#", @" WHERE cas.RepresentsASwitch <> 'Y' ");
                    break;

                case VariationReportOptions.InOutBoundMinutes:

                    break;

                case VariationReportOptions.TopDestinationMinutes:
                    query.Replace("#NameColumn#", " Z.Name ");
                    query.Replace("#ValueColumn#", " SUM(BS.SaleDuration)/60 ");
                    query.Replace("#IDColumn#", " Z.ZoneID ");
                    query.Replace("#JoinStatement#", @" JOIN Zone Z With(Nolock) ON Z.ZoneID=BS.SaleZoneID ");

                    query.Replace("#WhereStatement#", @" ");
                    break;

                case VariationReportOptions.InBoundAmount:
                    query.Replace("#NameColumn#", " cpc.Name ");
                    query.Replace("#ValueColumn#", " SUM(Sale_Nets) ");
                    query.Replace("#IDColumn#", " cac.CarrierAccountID ");
                    query.Replace("#JoinStatement#", @" JOIN CarrierAccount cac With(Nolock) ON cac.CarrierAccountID=BS.CustomerID
                                                        JOIN CarrierProfile cpc With(Nolock) ON cpc.ProfileID = cac.ProfileID ");
                    query.Replace("#WhereStatement#", @" WHERE cas.RepresentsASwitch <> 'Y' ");
                    break;
                case VariationReportOptions.OutBoundAmount:
                    query.Replace("#NameColumn#", " cps.Name ");
                    query.Replace("#ValueColumn#", " SUM(Cost_Nets) ");
                    query.Replace("#IDColumn#", " cas.CarrierAccountID ");
                    query.Replace("#JoinStatement#", @" JOIN CarrierAccount cas With(Nolock) ON cas.CarrierAccountID=BS.CustomerID
                                                        JOIN CarrierProfile cps With(Nolock) ON cps.ProfileID = cas.ProfileID ");
                    query.Replace("#WhereStatement#", @" WHERE cas.RepresentsASwitch <> 'Y' ");
                    break;

                case VariationReportOptions.InOutBoundAmount:
                    break;
                case VariationReportOptions.TopDestinationAmount:
                    break;
                case VariationReportOptions.Profit:
                    break;

            }


            if (query.ToString().Contains("@BeginTime") && query.ToString().Contains("@EndTime"))
            {
                query.Replace("@BeginTime", BeginTime.ToString());
                query.Replace("@EndTime", EndTime.ToString());
            }
            return query.ToString();
        }

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


        #endregion
    }
}
