using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Analytics.Entities;
using TOne.WhS.Analytics.Entities.BillingReport;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace TOne.WhS.Analytics.Data.SQL
{
    public class BillingReportDataManager : BaseSQLDataManager, IBillingReportDataManager
    {
                
        #region Constructors

        public BillingReportDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneV2DBConnString"))
        {

        }
        
        #endregion

        public List<Entities.BillingReport.BusinessCaseStatus> GetBusinessCaseStatus(DateTime fromDate, DateTime toDate, int carrierAccountId, int topDestination, bool isSale, bool isAmount, int currencyId)
        {
            string DurationField = "BS.SaleDuration / 60.0";
            string amountField = isSale ? "BS.Sale_Nets / ISNULL(ERS.Rate, 1) " : "BS.Cost_Nets / ISNULL(ERC.Rate, 1) ";
            string exchangeTable = isSale ? "ERS" : "ERC";
            string currencyField = isSale ? "Sale_Currency" : "Cost_Currency";
            string amountDuration = isAmount ? amountField : DurationField;
            string carrier = isSale ? "Customer" : "Supplier";
            string carrierId = isSale ? "BS.CustomerID" : "BS.SupplierID";
            string saleZone = isSale ? "BS.SaleZoneID" : "BS.CostZoneID";
            string query = String.Format(@"{4} ;WITH OrderedZones AS (SELECT TOP (@TopDestination) z.ZoneID, z.Name 
                From Billing_Stats BS WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_{1})) , @ExchangeRates as {5},
                Zone z (NOLOCK) WHERE bs.CallDate 
                BETWEEN @FromDate AND @ToDate AND {2} = @CustomerId AND z.ZoneID = {3} AND {5}.Currency = BS.{6}  
                GROUP BY z.ZoneID, z.Name 
                ORDER BY SUM({0}) DESC ) 
                
                SELECT z.Name,Year(bs.CallDate) AS YearDuration, 
                MONTH(BS.CallDate) AS MonthDuration, 
                cast( (SUM({0} )/60 ) as decimal(13,4) ) AS SaleDuration 
                From Billing_Stats BS WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_{1})), OrderedZones z , @ExchangeRates as ERC, @ExchangeRates as ERS

                WHERE bs.CallDate BETWEEN @FromDate AND @ToDate AND {2} = @CustomerId AND z.ZoneID = {3} 
                And ERC.Currency = BS.Cost_Currency AND ERC.Date = BS.CallDate 
                And ERS.Currency = BS.Sale_Currency AND ERS.Date = BS.CallDate
                GROUP BY z.Name , Year(bs.CallDate), MONTH(BS.CallDate)",
                amountDuration,
                carrier,
                carrierId,
                saleZone,
                //CurrencyQuery(fromDate, toDate, currencyId),
                GetExchangeRatesTable(currencyId),
                exchangeTable,
                currencyField
                );

            return GetItemsText(query, BusinessCaseStatusMapper,
            (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@TopDestination", topDestination));
                cmd.Parameters.Add(new SqlParameter("@FromDate", new DateTime(fromDate.Year, fromDate.Month, fromDate.Day)));
                cmd.Parameters.Add(new SqlParameter("@ToDate", new DateTime(toDate.Year, toDate.Month, toDate.Day)));
                cmd.Parameters.Add(new SqlParameter("@CustomerId", carrierAccountId));
            });
        }

        string GetExchangeRatesTable(int? currencyId)
        {
            if (currencyId.HasValue)
                return String.Format
                (
                    @"DECLARE @ConvertedExchangeRates TABLE
                    (
	                    CurrencyID INT NOT NULL,
	                    Rate DECIMAL(18, 6) NOT NULL,
	                    BED DATETIME NOT NULL,
	                    EED DATETIME,
	                    PRIMARY KEY(CurrencyID, BED)
                    )

                    INSERT INTO @ConvertedExchangeRates
                    SELECT * FROM Common.getExchangeRatesConvertedToCurrency({0}, @SmallestFromDate, @LargestToDate)",
                    currencyId
                );
            return null;
        }

        
        #region PrivatMethods
        private BusinessCaseStatus BusinessCaseStatusMapper(IDataReader reader)
        {
            BusinessCaseStatus instance = new BusinessCaseStatus
            {
                Zone = reader["Name"] as string,
                Month = GetReaderValue<int>(reader, "MonthDuration"),
                Year = GetReaderValue<int>(reader, "YearDuration"),
                Durations = GetReaderValue<decimal>(reader, "SaleDuration")
            };
            return instance;
        }
        #endregion
    }
}
