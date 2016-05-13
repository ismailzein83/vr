using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Analytics.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.Analytics.Data.SQL
{
    public class GenericBillingDataManager : BaseSQLDataManager, IGenericBillingDataManager
    {
        #region Fields / Constructors

        BillingReportQuery _inputQuery;

        #endregion

        #region Public Methods
        public IEnumerable<Entities.BillingReportRecord> GetFilteredBillingReportRecords(Vanrise.Entities.DataRetrievalInput<Entities.BillingReportQuery> input)
        {
            _inputQuery = input.Query;

            return GetItemsText<BillingReportRecord>(GetBillingReportQuery(), BillingReportRecordMapper, (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@FromDate", input.Query.FromDate));
                cmd.Parameters.Add(new SqlParameter("@ToDate", input.Query.ToDate));
                cmd.Parameters.Add(new SqlParameter("@@CurrencyID", input.Query.CurrencyId));
            });
        }
        #endregion


        #region Billing Report Query
        string GetBillingReportQuery()
        {
            var billingReportQueryBuilder = new StringBuilder
            (@"
                #EXCHANGE_RATES_TABLE#
                #QUERY_PART#
            ");

            billingReportQueryBuilder.Replace("#EXCHANGE_RATES_TABLE#", GetExchangeRatesTable());

            string queryPart = GetSingleBoundQuery();

            billingReportQueryBuilder.Replace("#QUERY_PART#", queryPart);

            return billingReportQueryBuilder.ToString();
        }

        string GetExchangeRatesTable()
        {
            return @"DECLARE @ExchangeRates TABLE
                    (
	                    CurrencyID INT NOT NULL,
	                    Rate DECIMAL(18, 6) NOT NULL,
	                    BED DATETIME NOT NULL,
	                    EED DATETIME,
	                    PRIMARY KEY(CurrencyID, BED)
                    )
                    INSERT INTO @ExchangeRates
                    SELECT * FROM Common.getExchangeRates(@SmallestFromDate, @LargestToDate)";
        }

        string GetSingleBoundQuery()
        {
            var singleResultQueryBuilder = new StringBuilder
            (@"
                SELECT
				bs.SaleZoneID AS SaleZoneID,
				SUM(bs.NumberOfCalls) AS Calls,
				AVG(ISNULL(bs.Cost_Rate/ISNULL(ERC.Rate, 1),0)) AS Rate,
				SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS DurationNet,
				bs.Cost_RateType AS RateType,
				SUM(bs.CostDuration/60.0) AS DurationInSeconds,
				SUM(bs.cost_nets /ISNULL(ERC.Rate, 1)) AS  Net,
				SUM(bs.Cost_Commissions/ISNULL(ERC.Rate, 1)) AS CommissionValue,
				SUM(bs.Cost_ExtraCharges/ISNULL(ERC.Rate, 1)) AS ExtraChargeValue

                FROM Billing_Stats bs WITH(NOLOCK,INDEX(IX_Billing_Stats_Date))
                #JOIN_PART#

                #WHERE_PART#

                GROUP BY #DIMENTION_COLUMN_NAME#
                #ORDER_BY_PART#

                #SELECT_FROM_RESULT_PART#

                #SUMMARY_PART#
            ");

            singleResultQueryBuilder.Replace("#JOIN_PART#", GetJoinPart());
            singleResultQueryBuilder.Replace("#WHERE_PART#", GetWherePart());
            singleResultQueryBuilder.Replace("#DIMENTION_COLUMN_NAME#", GetDimentionColumnName());
            singleResultQueryBuilder.Replace("#ORDER_BY_PART#", GetOrderByPart());
            singleResultQueryBuilder.Replace("#SUMMARY_PART#", null);
            
            return singleResultQueryBuilder.ToString();
        }


        string GetJoinPart()
        {
            StringBuilder joinPartBuilder = new StringBuilder();
            joinPartBuilder.Append(" LEFT JOIN @ExchangeRates ERC ON ERC.CurrencyIn = bs.Cost_Currency  AND ERC.CurrencyOut = @CurrencyID AND ERC.Date = bs.CallDate ");
            return joinPartBuilder.ToString();
        }

        string GetWherePart()
        {
            var wherePartBuilder = new StringBuilder(" bs.calldate >=@FromDate AND bs.calldate<=@ToDate");
            return wherePartBuilder.ToString();
        }
        string GetDimentionColumnName()
        {
            var wherePartBuilder = new StringBuilder(" bs.SaleZoneID,bs.Cost_RateType ");
            return wherePartBuilder.ToString();
        }
        string GetOrderByPart()
        {
            var wherePartBuilder = new StringBuilder(" bs.SaleZoneID ASC   ");
            return wherePartBuilder.ToString();
        }
        #endregion

        #region Private Methods

        BillingReportRecord BillingReportRecordMapper(IDataReader reader)
        {
            var billingReportRecord = new BillingReportRecord()
            {
                Zone = reader["Zone"] as string
            };

            return billingReportRecord;
        }

        #endregion
    }
}
