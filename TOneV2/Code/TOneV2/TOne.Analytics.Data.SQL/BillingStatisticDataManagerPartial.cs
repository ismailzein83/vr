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

        public string GetVariationReportQuery(DateTime selectedDate, int periodCount, TimePeriod TimePeriodEnum, VariationReportOptions VariationReportOptionsEnum)
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
           // foreach(var variationReportOptions in Enum.GetValues(VariationReportOptionsEnum.GetType()))
            switch (VariationReportOptionsEnum.ToString())
                {
                case "0":
                    query.Replace("#NameColumn#", " cpc.Name ");
                    query.Replace("#ValueColumn#"," SUM(SaleDuration) ");
                    query.Replace("#IDColumn#", " cac.CarrierAccountID as ID " );
                    query.Replace("#JoinStatement#", @" JOIN CarrierAccount cac With(Nolock) ON cac.CarrierAccountID=BS.CustomerID
                                                        JOIN CarrierProfile cpc With(Nolock) ON cpc.ProfileID = cac.ProfileID ");
                    query.Replace("#WhereStatement#"," WHERE CallDate BETWEEN DATEADD(@TimePeriod, -@PeriodCount+1, @FromDate) AND @FromDate ");
                    break;
                case "1": break;
                case "2": break;
                case "3": break;
                case "4": break;
                case "5": break;
                case "6": break;
                case "7": break;
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
                AccountId = reader["AccountID"] as string ,
                Services = GetReaderValue<double>(reader, "Services")
            };
            return instance;
        }
        #endregion
    }
}
