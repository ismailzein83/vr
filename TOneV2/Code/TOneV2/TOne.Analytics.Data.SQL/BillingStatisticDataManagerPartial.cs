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

        public string GetVariationReportQuery(DateTime selectedDate, int periodCount, TimePeriod timePeriod, VariationReportOptions variationReportOptions)
        {
            StringBuilder query = new StringBuilder(query_Common);
            query.Append(@" SELECT  #NameColumn# , 
            0.0 as [AVG],
            0.0 as [%], 
            0.0 as [Prev %],
            CallDate,
            (#ValueColumn#)/60 as Total,
            #IDColumn# as ID
            From Billing_Stats BS With(Nolock,INDEX(IX_Billing_Stats_Date))
            LEFT JOIN @ExchangeRates ERC ON ERC.Currency = BS.Cost_Currency AND ERC.Date = BS.CallDate			
            LEFT JOIN @ExchangeRates ERS ON ERS.Currency = BS.Sale_Currency AND ERS.Date = BS.CallDate
            #JoinStatement#
            #WhereStatement#  
            GROUP BY #NameColumn# ,#IDColumn#, CallDate");
            switch (variationReportOptions)
            {
                case VariationReportOptions.InBoundMinutes:
                    query.Replace("#NameColumn#", " cpc.Name ");
                    query.Replace("#ValueColumn#", " SUM(SaleDuration) ");
                    query.Replace("#IDColumn#", " cac.CarrierAccountID ");
                    query.Replace("#JoinStatement#", @" JOIN CarrierAccount cac With(Nolock) ON cac.CarrierAccountID=BS.CustomerID
                                                        JOIN CarrierProfile cpc With(Nolock) ON cpc.ProfileID = cac.ProfileID ");
                    query.Replace("#WhereStatement#", " WHERE CallDate BETWEEN DATEADD(@TimePeriod, -@PeriodCount+1, @FromDate) AND @FromDate ");
                    break;

                case VariationReportOptions.OutBoundMinutes:
                    query.Replace("#NameColumn#", " cps.Name ");
                    query.Replace("#ValueColumn#", " SUM(CostDuration) ");
                    query.Replace("#IDColumn#", " cas.CarrierAccountID ");
                    query.Replace("#JoinStatement#", @" JOIN CarrierAccount cas With(Nolock) ON cas.CarrierAccountID=BS.SupplierID
                                                        JOIN CarrierProfile cps With(Nolock) ON cps.ProfileID = cas.ProfileID ");
                    query.Replace("#WhereStatement#", @" CallDate BETWEEN DATEADD(@TimePeriod, -@PeriodCount+1, @FromDate) AND @FromDate
                                                        AND  cas.RepresentsASwitch <> 'Y' ");
                    break;

                case VariationReportOptions.InOutBoundMinutes:

                    break;

                case VariationReportOptions.TopDestinationMinutes:
                    query.Replace("#NameColumn#", " Z.Name ");
                    query.Replace("#ValueColumn#", " SUM(BS.SaleDuration) ");
                    query.Replace("#IDColumn#", " Z.ZoneID ");
                    query.Replace("#JoinStatement#", @" JOIN Zone Z With(Nolock) ON Z.ZoneID=BS.SaleZoneID ");

                    query.Replace("#WhereStatement#", @" WHERE CallDate BETWEEN DATEADD(@TimePeriod, -@PeriodCount+1, @FromDate) AND @FromDate ");
                    break;

                case VariationReportOptions.InBoundAmount:
                    query.Replace("#NameColumn#", " cpc.Name ");
                    query.Replace("#ValueColumn#", " SUM(Sale_Nets) ");
                    query.Replace("#IDColumn#", " cac.CarrierAccountID ");
                    query.Replace("#JoinStatement#", @" JOIN CarrierAccount cac With(Nolock) ON cac.CarrierAccountID=BS.CustomerID
                                                        JOIN CarrierProfile cpc With(Nolock) ON cpc.ProfileID = cac.ProfileID ");
                    query.Replace("#WhereStatement#", @" CallDate BETWEEN DATEADD(@TimePeriod, -@PeriodCount+1, @FromDate) AND @FromDate
                                                        AND  cas.RepresentsASwitch <> 'Y' ");
                    break;
                case VariationReportOptions.OutBoundAmount:
                    query.Replace("#NameColumn#", " cps.Name ");
                    query.Replace("#ValueColumn#", " SUM(Cost_Nets) ");
                    query.Replace("#IDColumn#", " cas.CarrierAccountID ");
                    query.Replace("#JoinStatement#", @" JOIN CarrierAccount cas With(Nolock) ON cas.CarrierAccountID=BS.CustomerID
                                                        JOIN CarrierProfile cps With(Nolock) ON cps.ProfileID = cas.ProfileID ");
                    query.Replace("#WhereStatement#", @" CallDate BETWEEN DATEADD(@TimePeriod, -@PeriodCount+1, @FromDate) AND @FromDate
                                                        AND  cas.RepresentsASwitch <> 'Y' ");
                    break;

                case VariationReportOptions.InOutBoundAmount:
                    break;
            }

            if (query.ToString().Contains("@TimePeriod"))
                switch (timePeriod)
                {
                    case TimePeriod.Days:  query.Replace("@TimePeriod", " DAY ");    break;
                    case TimePeriod.Weeks: query.Replace("@TimePeriod", " WEEK ");    break;
                    case TimePeriod.Months:query.Replace("@TimePeriod", " MONTH ");   break;

                }
            return query.ToString();
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
        #endregion
    }
}
