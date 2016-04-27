using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Analytics.Entities;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace TOne.WhS.Analytics.Data.SQL
{
    public class VariationReportDataManager : BaseSQLDataManager, IVariationReportDataManager
    {
        #region Fields / Constructors

        VariationReportQuery _inputQuery;

        static Dictionary<string, string> s_mapper;

        static VariationReportDataManager()
        {
            s_mapper = new Dictionary<string, string>();
            s_mapper.Add("DimensionName", "DimensionId");
        }

        #endregion

        #region Public Methods

        public IEnumerable<VariationReportRecord> GetFilteredVariationReportRecords(DataRetrievalInput<VariationReportQuery> input, DataTable timePeriodsTable)
        {
            _inputQuery = input.Query;

            return GetItemsText<VariationReportRecord>(GetVariationReportQuery(input.Query), VariationReportRecordMapper, (cmd) =>
            {
                var tableParameter = new SqlParameter("@TimePeriods", SqlDbType.Structured);
                tableParameter.TypeName = "TOneWhS_Analytic.VariationReportTimePeriodType";
                tableParameter.Value = timePeriodsTable;
                cmd.Parameters.Add(tableParameter);

                cmd.Parameters.Add(new SqlParameter("@SmallestFromDate", GetSmallestFromDate(timePeriodsTable)));
                cmd.Parameters.Add(new SqlParameter("@LargestToDate", input.Query.ToDate.AddDays(1)));
            });
        }

        #endregion

        #region Private Methods

        #region Variation Report Query

        string GetVariationReportQuery(VariationReportQuery query)
        {
            var variationReportQueryBuilder = new StringBuilder
            (@"
                #EXCHANGE_RATES_TABLE#
                #QUERY_PART#
            ");

            variationReportQueryBuilder.Replace("#EXCHANGE_RATES_TABLE#", GetExchangeRatesTable(query));

            string queryPart = (query.ReportType == VariationReportType.InOutBoundMinutes || query.ReportType == VariationReportType.InOutBoundAmount) ?
                GetMultiBoundQuery(query) :
                GetSingleBoundQuery(query, null, "#Result", true, true);

            variationReportQueryBuilder.Replace("#QUERY_PART#", queryPart);

            return variationReportQueryBuilder.ToString();
        }

        string GetExchangeRatesTable(VariationReportQuery query)
        {
            switch (query.ReportType)
            {
                case VariationReportType.InBoundAmount:
                case VariationReportType.OutBoundAmount:
                case VariationReportType.InOutBoundAmount:
                case VariationReportType.TopDestinationAmount:
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
                default:
                    return null;
            }
        }

        #region Multi Bound Query

        string GetMultiBoundQuery(VariationReportQuery query)
        {
            var multiBoundQueryBuilder = new StringBuilder
            (@"
                #IN_BOUND_QUERY#
                #OUT_BOUND_QUERY#

                SELECT InResult.DimensionId,
                    ' / Total' AS DimensionSuffix,
                    InResult.Average - OutResult.Average Average,
                    InResult.Percentage - OutResult.Percentage Percentage,
                    InResult.PreviousPeriodPercentage - OutResult.PreviousPeriodPercentage PreviousPeriodPercentage
                    #TOTAL_RESULT_SUM_PART#
                INTO #TotalResult
                FROM #InResult InResult JOIN #OutResult OutResult ON InResult.DimensionId = OutResult.DimensionId

                SELECT *
                FROM (SELECT * FROM #InResult UNION ALL SELECT * FROM #OutResult UNION ALL SELECT * FROM #TotalResult) AS FinalResult
                
                ORDER BY FinalResult.DimensionId
            ");

            VariationReportQuery inResultQuery;
            VariationReportQuery outResultQuery;
            SetInOutResultQueries(query, out inResultQuery, out outResultQuery);

            multiBoundQueryBuilder.Replace("#IN_BOUND_QUERY#", GetSingleBoundQuery(inResultQuery, " / In", "#InResult", false, false));
            multiBoundQueryBuilder.Replace("#OUT_BOUND_QUERY#", GetSingleBoundQuery(outResultQuery, " / Out", "#OutResult", false, false));
            multiBoundQueryBuilder.Replace("#TOTAL_RESULT_SUM_PART#", GetTotalResultSumPart(query));

            return multiBoundQueryBuilder.ToString();
        }

        void SetInOutResultQueries(VariationReportQuery query, out VariationReportQuery inResultQuery, out VariationReportQuery outResultQuery)
        {
            if (query.ReportType == VariationReportType.InOutBoundMinutes)
            {
                inResultQuery = new VariationReportQuery() { ReportType = VariationReportType.InBoundMinutes };
                outResultQuery = new VariationReportQuery() { ReportType = VariationReportType.OutBoundMinutes };
            }
            else // VariationReportType.InOutBoundAmount
            {
                inResultQuery = new VariationReportQuery() { ReportType = VariationReportType.InBoundAmount };
                outResultQuery = new VariationReportQuery() { ReportType = VariationReportType.OutBoundAmount };
            }

            inResultQuery.ToDate = query.ToDate;
            inResultQuery.TimePeriod = query.TimePeriod;
            inResultQuery.NumberOfPeriods = query.NumberOfPeriods;
            inResultQuery.DimensionFilters = query.DimensionFilters;

            outResultQuery.ToDate = query.ToDate;
            outResultQuery.TimePeriod = query.TimePeriod;
            outResultQuery.NumberOfPeriods = query.NumberOfPeriods;
            outResultQuery.DimensionFilters = query.DimensionFilters;
        }

        string GetTotalResultSumPart(VariationReportQuery query)
        {
            var totalResultSumPartBuilder = new StringBuilder();
            for (int i = 0; i < query.NumberOfPeriods; i++)
            {
                totalResultSumPartBuilder.Append(String.Format(", InResult.Period{0}Sum - OutResult.Period{0}Sum AS Period{0}Sum", i + 1));
            }
            return totalResultSumPartBuilder.ToString();
        }

        #endregion

        #region Single Bound Query

        string GetSingleBoundQuery(VariationReportQuery query, string dimensionSuffix, string intoTableName, bool selectFromIntoTable, bool withSummary)
        {
            var singleResultQueryBuilder = new StringBuilder
            (@"
                SELECT #TOP_PART#
                    #DIMENTION_COLUMN_NAME# AS DimensionId
                    #DIMENSION_SUFFIX#
                    #AVERAGE_PART#
                    #PERCENTAGE_PART#
                    #PREVIOUS_PERIOD_PERCENTAGE_PART#
                    #SUM_PART#

                INTO #INTO_TABLE_NAME#

                FROM TOneWhs_Analytic.BillingStats bs
                #JOIN_PART#

                #WHERE_PART#

                GROUP BY #DIMENTION_COLUMN_NAME#
                #ORDER_BY_PART#

                #SELECT_FROM_RESULT_PART#

                #SUMMARY_PART#
            ");

            singleResultQueryBuilder.Replace("#TOP_PART#", GetTopPart(query));
            singleResultQueryBuilder.Replace("#DIMENTION_COLUMN_NAME#", GetDimentionColumnName(query));
            singleResultQueryBuilder.Replace("#DIMENSION_SUFFIX#", String.Format(", '{0}' AS DimensionSuffix", dimensionSuffix));

            singleResultQueryBuilder.Replace("#AVERAGE_PART#", GetAveragePart(query));
            singleResultQueryBuilder.Replace("#PERCENTAGE_PART#", GetPercentagePart(query));
            singleResultQueryBuilder.Replace("#PREVIOUS_PERIOD_PERCENTAGE_PART#", GetPreviousPeriodPercentagePart(query));

            singleResultQueryBuilder.Replace("#SUM_PART#", GetSumPart(query));
            singleResultQueryBuilder.Replace("#INTO_TABLE_NAME#", intoTableName);

            singleResultQueryBuilder.Replace("#JOIN_PART#", GetJoinPart(query));
            singleResultQueryBuilder.Replace("#WHERE_PART#", GetWherePart(query));
            singleResultQueryBuilder.Replace("#ORDER_BY_PART#", GetOrderByPart(query));

            singleResultQueryBuilder.Replace("#SELECT_FROM_RESULT_PART#", (selectFromIntoTable) ? String.Format("SELECT * FROM {0}", intoTableName) : null);
            singleResultQueryBuilder.Replace("#SUMMARY_PART#", null); // GetSummaryPart(query.NumberOfPeriods)

            return singleResultQueryBuilder.ToString();
        }

        string GetTopPart(VariationReportQuery query)
        {
            return null;
            //return (reportType == VariationReportType.TopDestinationMinutes || reportType == VariationReportType.TopDestinationAmount) ? "TOP 5 " : null;
        }

        string GetDimentionColumnName(VariationReportQuery query)
        {
            string dimensionColumnName = null;
            switch (query.ReportType)
            {
                case VariationReportType.InBoundMinutes:
                case VariationReportType.InBoundAmount:
                    dimensionColumnName = "CustomerID";
                    break;
                case VariationReportType.OutBoundMinutes:
                case VariationReportType.OutBoundAmount:
                    dimensionColumnName = "SupplierID";
                    break;
                case VariationReportType.TopDestinationMinutes:
                case VariationReportType.TopDestinationAmount:
                    dimensionColumnName = "SaleZoneID";
                    break;
            }
            return dimensionColumnName;
        }

        string GetAveragePart(VariationReportQuery query)
        {
            return String.Format(", SUM({0}){1} / {2} AS Average", GetSumExpression(query), GetSumExpressionExtension(query), query.NumberOfPeriods);
        }

        string GetPercentagePart(VariationReportQuery query)
        {
            return String.Format
            (
                ", ((SUM(CASE WHEN p1.FromDate IS NOT NULL THEN {0} ELSE 0 END){1}) - (SUM({0}){1} / {2})) / dbo.ZeroToMax(SUM({0}){1} / {2}) * 100 AS Percentage",
                GetSumExpression(query), GetSumExpressionExtension(query),
                query.NumberOfPeriods
            );
        }

        string GetPreviousPeriodPercentagePart(VariationReportQuery query)
        {
            return (query.NumberOfPeriods > 1) ?
                String.Format
                (
                    @", ((SUM(CASE WHEN p1.FromDate IS NOT NULL THEN {0} ELSE 0 END){1}) - (SUM(CASE WHEN p2.FromDate IS NOT NULL THEN {0} ELSE 0 END){1})) /
                        dbo.ZeroToMax(SUM(CASE WHEN p2.FromDate IS NOT NULL THEN {0} ELSE 0 END){1}) * 100 AS PreviousPeriodPercentage",
                    GetSumExpression(query),
                    GetSumExpressionExtension(query)
                ) : null;
        }

        string GetSumPart(VariationReportQuery query)
        {
            StringBuilder sumPartBuilder = new StringBuilder();
            string sumExpression = GetSumExpression(query);
            string sumExpressionExtension = GetSumExpressionExtension(query);
            for (int i = 0; i < query.NumberOfPeriods; i++)
            {
                sumPartBuilder.Append(String.Format(", SUM(CASE WHEN p{0}.FromDate IS NOT NULL THEN {1} ELSE 0 END){2} Period{0}Sum", i + 1, sumExpression, sumExpressionExtension));
            }
            return sumPartBuilder.ToString();
        }

        #region Sum Expression

        string GetSumColumnName(VariationReportQuery query)
        {
            string sumColumnName = null;

            if (query.ParentReportType != null)
            {
                var parentReportType = (VariationReportType)query.ParentReportType;

                // Parent is a customer
                if (parentReportType == VariationReportType.InBoundMinutes || parentReportType == VariationReportType.InBoundAmount)
                {
                    // Children are zones
                    if (query.ReportType == VariationReportType.TopDestinationMinutes)
                        sumColumnName = "SaleDuration";
                    else if (query.ReportType == VariationReportType.TopDestinationAmount)
                        sumColumnName = "SaleNets";
                }
                // Parent is a supplier
                else if (parentReportType == VariationReportType.OutBoundMinutes || parentReportType == VariationReportType.OutBoundAmount)
                {
                    // Children are zones
                    if (query.ReportType == VariationReportType.TopDestinationMinutes)
                        sumColumnName = "CostDuration";
                    else if (query.ReportType == VariationReportType.TopDestinationAmount)
                        sumColumnName = "CostNets";
                }
                // Parent is a zone
                else if (parentReportType == VariationReportType.TopDestinationMinutes || parentReportType == VariationReportType.TopDestinationAmount)
                {
                    // Children are customers or suppliers
                    if (query.ReportType == VariationReportType.InBoundMinutes || query.ReportType == VariationReportType.OutBoundMinutes)
                        sumColumnName = "SaleDuration";
                    else if (query.ReportType == VariationReportType.InBoundAmount || query.ReportType == VariationReportType.OutBoundAmount)
                        sumColumnName = "SaleNets";
                }
                // Parent is a customer or a supplier
                else if (parentReportType == VariationReportType.InOutBoundMinutes || parentReportType == VariationReportType.InOutBoundAmount)
                {
                    // Children are zones

                    VariationReportDimension parentDimension = query.DimensionFilters.ElementAt(0).Dimension;
                    
                    if (parentDimension == VariationReportDimension.Customer)
                    {
                        if (query.ReportType == VariationReportType.TopDestinationMinutes)
                            sumColumnName = "SaleDuration";
                        else if (query.ReportType == VariationReportType.TopDestinationAmount)
                            sumColumnName = "SaleNets";
                    }
                    else if (parentDimension == VariationReportDimension.Supplier)
                    {
                        if (query.ReportType == VariationReportType.TopDestinationMinutes)
                            sumColumnName = "CostDuration";
                        else if (query.ReportType == VariationReportType.TopDestinationAmount)
                            sumColumnName = "CostNets";
                    }
                }
            }
            else
            {
                switch (query.ReportType)
                {
                    case VariationReportType.InBoundMinutes:
                    case VariationReportType.TopDestinationMinutes:
                        sumColumnName = "SaleDuration";
                        break;
                    case VariationReportType.OutBoundMinutes:
                        sumColumnName = "CostDuration";
                        break;
                    case VariationReportType.InBoundAmount:
                    case VariationReportType.TopDestinationAmount:
                        sumColumnName = "SaleNets";
                        break;
                    case VariationReportType.OutBoundAmount:
                        sumColumnName = "CostNets";
                        break;
                }
            }

            return sumColumnName;
        }

        string GetSumExpression(VariationReportQuery query)
        {
            string sumColumnName = GetSumColumnName(query);
            string sumExpression = sumColumnName;
            switch (query.ReportType)
            {
                case VariationReportType.InBoundAmount:
                case VariationReportType.TopDestinationAmount:
                    sumExpression = String.Format("({0} / ISNULL(SER.Rate, 1))", sumColumnName);
                    break;
                case VariationReportType.OutBoundAmount:
                    sumExpression = String.Format("({0} / ISNULL(CER.Rate, 1))", sumColumnName);
                    break;
            }
            return sumExpression;
        }

        string GetSumExpressionExtension(VariationReportQuery query)
        {
            string sumExpressionExtension;
            switch (query.ReportType)
            {
                case VariationReportType.InBoundMinutes:
                case VariationReportType.OutBoundMinutes:
                case VariationReportType.TopDestinationMinutes:
                    sumExpressionExtension = " / 60";
                    break;
                default:
                    sumExpressionExtension = null;
                    break;
            }
            return sumExpressionExtension;
        }

        #endregion

        string GetJoinPart(VariationReportQuery query)
        {
            StringBuilder joinPartBuilder = new StringBuilder();
            switch (query.ReportType)
            {
                case VariationReportType.InBoundAmount:
                case VariationReportType.OutBoundAmount:
                case VariationReportType.TopDestinationAmount:
                    joinPartBuilder.Append(String.Format(" LEFT JOIN @ExchangeRates SER on bs.SaleCurrency = SER.CurrencyID AND bs.CallDate >= SER.BED AND (SER.EED IS NULL OR bs.CallDate < SER.EED)"));
                    joinPartBuilder.Append(String.Format(" LEFT JOIN @ExchangeRates CER on bs.CostCurrency = CER.CurrencyID AND bs.CallDate >= CER.BED AND (CER.EED IS NULL OR bs.CallDate < CER.EED)"));
                    break;
            }
            for (int i = 0; i < query.NumberOfPeriods; i++)
            {
                joinPartBuilder.Append(String.Format(" LEFT JOIN @TimePeriods p{0} on bs.CallDate >= p{0}.FromDate AND bs.CallDate < p{0}.ToDate AND p{0}.PeriodIndex = {0}", i + 1));
            }
            return joinPartBuilder.ToString();
        }

        #region Where Part

        string GetWherePart(VariationReportQuery query)
        {
            var wherePartBuilder = new StringBuilder("WHERE bs.CallDate >= @SmallestFromDate AND CallDate < @LargestToDate");
            if (query.DimensionFilters != null)
            {
                // The loop doesn't account for filter values that should be enclosed using single quotes
                foreach (VariationReportDimensionFilter dimensionFilter in query.DimensionFilters)
                    wherePartBuilder.Append(String.Format(" AND {0} IN ({1})", GetDimensionColumnName(dimensionFilter.Dimension), String.Join(",", dimensionFilter.FilterValues)));
            }
            return wherePartBuilder.ToString();
        }

        string GetDimensionColumnName(VariationReportDimension dimension)
        {
            switch (dimension)
            {
                case VariationReportDimension.Customer:
                    return "CustomerID";
                case VariationReportDimension.Supplier:
                    return "SupplierID";
                case VariationReportDimension.Zone:
                    return "SaleZoneID";
            }
            throw new ArgumentException("dimension");
        }

        #endregion

        string GetOrderByPart(VariationReportQuery query)
        {
            string orderByExpression = String.Format("ORDER BY SUM({0}) DESC", GetSumColumnName(query));
            switch (query.ReportType)
            {
                case VariationReportType.InBoundAmount:
                case VariationReportType.OutBoundAmount:
                case VariationReportType.TopDestinationAmount:
                    orderByExpression = String.Format("ORDER BY SUM((SaleNets / ISNULL(SER.Rate, 1)) - (CostNets / ISNULL(CER.Rate, 1))) DESC");
                    break;
            }
            return orderByExpression;
        }

        #region Summary Part

        string GetSummaryPart(VariationReportQuery query)
        {
            var summaryPartBuilder = new StringBuilder
            (
                @"SELECT SUM(Average) AS Average,
                    (SUM(Period1Sum) - SUM(Average)) / dbo.ZeroToMax(SUM(Average)) * 100 AS Percentage,
	                (SUM(Period1Sum) - SUM(Period2Sum)) / dbo.ZeroToMax(SUM(Period2Sum)) * 100 AS PreviousPeriodPercentage
                    #PERIOD_SUMMARIES_PART#
                    FROM #Result"
            );

            summaryPartBuilder.Replace("#PERIOD_SUMMARIES_PART#", GetPeriodSummariesPart(query));
            return summaryPartBuilder.ToString();
        }

        string GetPeriodSummariesPart(VariationReportQuery query)
        {
            var periodSummariesPart = new StringBuilder();
            for (int i = 0; i < query.NumberOfPeriods; i++)
            {
                periodSummariesPart.AppendFormat(", SUM(Period{0}Sum) AS Period{0}Sum", i + 1);
            }
            return periodSummariesPart.ToString();
        }

        #endregion

        #endregion

        #endregion

        DateTime GetSmallestFromDate(DataTable timePeriodsTable)
        {
            DataRow lastRow = timePeriodsTable.Rows[timePeriodsTable.Rows.Count - 1];
            return Convert.ToDateTime(lastRow["FromDate"]);
        }

        #endregion

        #region Mappers

        VariationReportRecord VariationReportRecordMapper(IDataReader reader)
        {
            var variationReportRecord = new VariationReportRecord()
            {
                DimensionId = (object)reader["DimensionId"],
                DimensionSuffix = reader["DimensionSuffix"] as string,
                Average = (decimal)reader["Average"],
                Percentage = Convert.ToDecimal((double)reader["Percentage"]),
                PreviousPeriodPercentage = Convert.ToDecimal((double)reader["PreviousPeriodPercentage"])
            };

            if (_inputQuery != null)
            {
                variationReportRecord.TimePeriodValues = new List<decimal>();
                for (int i = 0; i < _inputQuery.NumberOfPeriods; i++)
                    variationReportRecord.TimePeriodValues.Add((decimal)reader[String.Format("Period{0}Sum", i + 1)]);
            }

            return variationReportRecord;
        }

        #endregion
    }
}
