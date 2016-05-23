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

        public GenericBillingDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneV2DBConnString"))
        { }
        
        #endregion

        #region Public Methods
        public List<Entities.BillingReportRecord> GetFilteredBillingReportRecords(Entities.BillingReportQuery input)
        {
            _inputQuery = input;

            return GetItemsText<BillingReportRecord>(GetBillingReportQuery(), BillingReportRecordMapper, (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@FromDate", input.FromDate));
                cmd.Parameters.Add(new SqlParameter("@ToDate", input.ToDate));
                cmd.Parameters.Add(new SqlParameter("@CurrencyID", input.CurrencyId));
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
                    SELECT * FROM Common.getExchangeRates(@FromDate, @ToDate)";
        }

        string GetSingleBoundQuery()
        {
            var singleResultQueryBuilder = new StringBuilder
            (@"
                SELECT
				bs.SaleZoneID AS SaleZoneID,
				SUM(bs.NumberOfCalls) AS Calls,
				AVG(ISNULL(bs.CostRate/ISNULL(ERC.Rate, 1),0)) AS Rate,
				SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS DurationNet,
				bs.CostRateType AS RateType,
				SUM(bs.CostDuration/60.0) AS DurationInSeconds,
				SUM(bs.CostNets /ISNULL(ERC.Rate, 1)) AS  Net,
				--SUM(bs.Cost_Commissions/ISNULL(ERC.Rate, 1)) AS CommissionValue,
				SUM(bs.CostExtraCharges/ISNULL(ERC.Rate, 1)) AS ExtraChargeValue

                FROM TOneWhS_Analytic.BillingStats bs WITH(NOLOCK,INDEX(IX_BillingStats_ID))
                #JOIN_PART#

                #WHERE_PART#

                GROUP BY #DIMENTION_COLUMN_NAME#
                #ORDER_BY_PART#

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
            joinPartBuilder.Append(" LEFT JOIN @ExchangeRates ERC ON ERC.CurrencyID = bs.CostCurrency ");
            return joinPartBuilder.ToString();
        }

        string GetWherePart()
        {
            var wherePartBuilder = new StringBuilder(" AND bs.calldate >=@FromDate AND bs.calldate<=@ToDate");
            return wherePartBuilder.ToString();
        }
        string GetDimentionColumnName()
        {
            var wherePartBuilder = new StringBuilder(" bs.SaleZoneID,bs.CostRateType ");
            return wherePartBuilder.ToString();
        }
        string GetOrderByPart()
        {
            var wherePartBuilder = new StringBuilder(" ORDER BY bs.SaleZoneID DESC   ");
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
