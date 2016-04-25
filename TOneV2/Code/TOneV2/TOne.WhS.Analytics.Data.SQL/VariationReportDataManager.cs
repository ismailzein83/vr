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
        string _dimensionTitle;

        static Dictionary<string, string> s_mapper;

        static VariationReportDataManager()
        {
            s_mapper = new Dictionary<string, string>();
            s_mapper.Add("DimensionName", "DimensionId");
        }

        #endregion

        #region Public Methods

        public VariationReportBigResult GetFilteredVariationReportRecords(DataRetrievalInput<VariationReportQuery> input)
        {
            _inputQuery = input.Query;

            Action<string> createTempTableAction = (tempTableName) =>
            {
                CreateTempTableIfNotExists(input, tempTableName);
            };

            BigResult<VariationReportRecord> bigResult = RetrieveData(input, createTempTableAction, VariationReportRecordMapper, s_mapper);

            VariationReportBigResult variationReportBigResult = new VariationReportBigResult()
            {
                ResultKey = bigResult.ResultKey,
                Data = bigResult.Data,
                TotalCount = bigResult.TotalCount
            };

            variationReportBigResult.DimensionTitle = _dimensionTitle;
            variationReportBigResult.TimePeriods = GetTimePeriodDefinitions(input);

            return variationReportBigResult;
        }

        #endregion

        #region Private Methods

        void CreateTempTableIfNotExists(DataRetrievalInput<VariationReportQuery> input, string tempTableName)
        {
            ExecuteNonQueryText(GetTempTableQuery(input.Query, tempTableName), (cmd) =>
            {
                DataTable timePeriodsTable = BuildTimePeriodsTable(input);
                var tableParameter = new SqlParameter("@TimePeriods", SqlDbType.Structured);

                tableParameter.TypeName = "TOneWhS_Analytic.VariationReportTimePeriodType";
                tableParameter.Value = timePeriodsTable;
                cmd.Parameters.Add(tableParameter);

                cmd.Parameters.Add(new SqlParameter("@SmallestFromDate", GetSmallestFromDate(timePeriodsTable)));
                cmd.Parameters.Add(new SqlParameter("@LargestToDate", input.Query.ToDate.AddDays(1)));
            });
        }

        #region Temp Table Query

        string GetTempTableQuery(VariationReportQuery query, string tempTableName)
        {
            var createTempTableQueryBuilder = new StringBuilder
            (
                @"IF NOT OBJECT_ID('#TEMP_TABLE_NAME#', N'U') IS NOT NULL
                BEGIN
                    #EXCHANGE_RATES_TABLE#
                    #QUERY_PART#
                END"
            );

            createTempTableQueryBuilder.Replace("#EXCHANGE_RATES_TABLE#", GetExchangeRatesTable(query.ReportType));

            string queryPart = (query.ReportType == VariationReportType.InOutBoundMinutes || query.ReportType == VariationReportType.InOutBoundAmount) ?
                GetMultiBoundQuery(query) :
                GetSingleBoundQuery(query, null, "INTO #TEMP_TABLE_NAME#");

            createTempTableQueryBuilder.Replace("#QUERY_PART#", queryPart);
            createTempTableQueryBuilder.Replace("#TEMP_TABLE_NAME#", tempTableName);

            return createTempTableQueryBuilder.ToString();
        }

        string GetExchangeRatesTable(VariationReportType reportType)
        {
            switch (reportType)
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

                SELECT * INTO #TEMP_TABLE_NAME#
                FROM (SELECT * FROM #InResult UNION ALL SELECT * FROM #OutResult UNION ALL SELECT * FROM #TotalResult) AS FinalResult
                
                ORDER BY FinalResult.DimensionId
            ");

            VariationReportQuery inResultQuery;
            VariationReportQuery outResultQuery;
            SetInOutResultQueries(query, out inResultQuery, out outResultQuery);

            multiBoundQueryBuilder.Replace("#IN_BOUND_QUERY#", GetSingleBoundQuery(inResultQuery, " / In", "INTO #InResult"));
            multiBoundQueryBuilder.Replace("#OUT_BOUND_QUERY#", GetSingleBoundQuery(outResultQuery, " / Out", "INTO #OutResult"));
            multiBoundQueryBuilder.Replace("#TOTAL_RESULT_SUM_PART#", GetTotalResultSumPart(query.NumberOfPeriods));

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

        string GetTotalResultSumPart(int numberOfPeriods)
        {
            var totalResultSumPartBuilder = new StringBuilder();
            for (int i = 0; i < numberOfPeriods; i++)
            {
                totalResultSumPartBuilder.Append(String.Format(", InResult.Period{0}Sum - OutResult.Period{0}Sum AS Period{0}Sum", i + 1));
            }
            return totalResultSumPartBuilder.ToString();
        }

        #endregion

        #region Single Bound Query

        string GetSingleBoundQuery(VariationReportQuery query, string dimensionSuffix, string intoPart)
        {
            var singleResultQueryBuilder = new StringBuilder
            (
                @"SELECT #TOP_PART#
                    #DIMENTION_COLUMN_NAME# AS DimensionId
                    #DIMENSION_SUFFIX#
                    #AVERAGE_PART#
                    #PERCENTAGE_PART#
                    #PREVIOUS_PERIOD_PERCENTAGE_PART#
                    #SUM_PART#

                #INTO_PART#

                FROM TOneWhs_Analytic.BillingStats bs
                #JOIN_PART#

                #WHERE_PART#

                GROUP BY #DIMENTION_COLUMN_NAME#
                #ORDER_BY_PART#"
            );

            singleResultQueryBuilder.Replace("#TOP_PART#", GetTopPart(query.ReportType));
            singleResultQueryBuilder.Replace("#DIMENTION_COLUMN_NAME#", GetDimentionColumnNameAndSetDimensionTitle(query.ReportType));
            singleResultQueryBuilder.Replace("#DIMENSION_SUFFIX#", String.Format(", '{0}' AS DimensionSuffix", dimensionSuffix));

            singleResultQueryBuilder.Replace("#AVERAGE_PART#", GetAveragePart(query.ReportType, query.NumberOfPeriods));
            singleResultQueryBuilder.Replace("#PERCENTAGE_PART#", GetPercentagePart(query.ReportType, query.NumberOfPeriods));
            singleResultQueryBuilder.Replace("#PREVIOUS_PERIOD_PERCENTAGE_PART#", GetPreviousPeriodPercentagePart(query.ReportType, query.NumberOfPeriods));

            singleResultQueryBuilder.Replace("#SUM_PART#", GetSumPart(query.ReportType, query.NumberOfPeriods));
            singleResultQueryBuilder.Replace("#INTO_PART#", intoPart);

            singleResultQueryBuilder.Replace("#JOIN_PART#", GetJoinPart(query.ReportType, query.NumberOfPeriods));
            singleResultQueryBuilder.Replace("#WHERE_PART#", GetWherePart(query.DimensionFilters));
            singleResultQueryBuilder.Replace("#ORDER_BY_PART#", GetOrderByPart(query.ReportType));

            return singleResultQueryBuilder.ToString();
        }

        string GetTopPart(VariationReportType reportType)
        {
            return (reportType == VariationReportType.TopDestinationMinutes || reportType == VariationReportType.TopDestinationAmount) ? "TOP 5 " : null;
        }

        string GetDimentionColumnNameAndSetDimensionTitle(VariationReportType reportType)
        {
            string dimensionColumnName;
            switch (reportType)
            {
                case VariationReportType.InBoundMinutes:
                case VariationReportType.InBoundAmount:
                    dimensionColumnName = "CustomerID";
                    _dimensionTitle = "Customer";
                    break;
                case VariationReportType.OutBoundMinutes:
                case VariationReportType.OutBoundAmount:
                    dimensionColumnName = "SupplierID";
                    _dimensionTitle = "Supplier";
                    break;
                case VariationReportType.TopDestinationMinutes:
                case VariationReportType.TopDestinationAmount:
                    dimensionColumnName = "SaleZoneID";
                    _dimensionTitle = "Sale Zone";
                    break;
                default:
                    throw new ArgumentException("reportType");
            }
            return dimensionColumnName;
        }

        string GetAveragePart(VariationReportType reportType, int numberOfPeriods)
        {
            return String.Format(", SUM({0}){1} / {2} AS Average", GetSumExpression(reportType), GetSumExpressionExtension(reportType), numberOfPeriods);
        }

        string GetPercentagePart(VariationReportType reportType, int numberOfPeriods)
        {
            return String.Format
            (
                ", ((SUM(CASE WHEN p1.FromDate IS NOT NULL THEN {0} ELSE 0 END){1}) - (SUM({0}){1} / {2})) / dbo.ZeroToMax(SUM({0}){1} / {2}) * 100 AS Percentage",
                GetSumExpression(reportType), GetSumExpressionExtension(reportType),
                numberOfPeriods
            );
        }

        string GetPreviousPeriodPercentagePart(VariationReportType reportType, int numberOfPeriods)
        {
            return (numberOfPeriods > 1) ?
                String.Format
                (
                    @", ((SUM(CASE WHEN p1.FromDate IS NOT NULL THEN {0} ELSE 0 END){1}) - (SUM(CASE WHEN p2.FromDate IS NOT NULL THEN {0} ELSE 0 END){1})) /
                        dbo.ZeroToMax(SUM(CASE WHEN p2.FromDate IS NOT NULL THEN {0} ELSE 0 END){1}) * 100 AS PreviousPeriodPercentage",
                    GetSumExpression(reportType),
                    GetSumExpressionExtension(reportType)
                ) : null;
        }

        string GetSumPart(VariationReportType reportType, int rowsCount)
        {
            StringBuilder sumPartBuilder = new StringBuilder();
            string sumExpression = GetSumExpression(reportType);
            string sumExpressionExtension = GetSumExpressionExtension(reportType);
            for (int i = 0; i < rowsCount; i++)
            {
                sumPartBuilder.Append(String.Format(", SUM(CASE WHEN p{0}.FromDate IS NOT NULL THEN {1} ELSE 0 END){2} Period{0}Sum", i + 1, sumExpression, sumExpressionExtension));
            }
            return sumPartBuilder.ToString();
        }

        #region Sum Expression

        string GetSumColumnName(VariationReportType reportType)
        {
            string sumColumnName;
            switch (reportType)
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

                default:
                    throw new NotImplementedException();
            }
            return sumColumnName;
        }

        string GetSumExpression(VariationReportType reportType)
        {
            string sumColumnName = GetSumColumnName(reportType);
            string sumExpression = sumColumnName;
            switch (reportType)
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

        string GetSumExpressionExtension(VariationReportType reportType)
        {
            string sumExpressionExtension;
            switch (reportType)
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

        string GetJoinPart(VariationReportType reportType, int rowsCount)
        {
            StringBuilder joinPartBuilder = new StringBuilder();
            switch (reportType)
            {
                case VariationReportType.InBoundAmount:
                case VariationReportType.OutBoundAmount:
                case VariationReportType.TopDestinationAmount:
                    joinPartBuilder.Append(String.Format(" LEFT JOIN @ExchangeRates SER on bs.SaleCurrency = SER.CurrencyID AND bs.CallDate >= SER.BED AND (SER.EED IS NULL OR bs.CallDate < SER.EED)"));
                    joinPartBuilder.Append(String.Format(" LEFT JOIN @ExchangeRates CER on bs.CostCurrency = CER.CurrencyID AND bs.CallDate >= CER.BED AND (CER.EED IS NULL OR bs.CallDate < CER.EED)"));
                    break;
            }
            for (int i = 0; i < rowsCount; i++)
            {
                joinPartBuilder.Append(String.Format(" LEFT JOIN @TimePeriods p{0} on bs.CallDate >= p{0}.FromDate AND bs.CallDate < p{0}.ToDate AND p{0}.PeriodIndex = {0}", i + 1));
            }
            return joinPartBuilder.ToString();
        }

        #region Where Part

        string GetWherePart(IEnumerable<VariationReportDimensionFilter> dimensionFilters)
        {
            var wherePartBuilder = new StringBuilder("WHERE bs.CallDate >= @SmallestFromDate AND CallDate < @LargestToDate");
            if (dimensionFilters != null)
            {
                // The loop doesn't account for filter values that should be enclosed using single quotes
                foreach (VariationReportDimensionFilter dimensionFilter in dimensionFilters)
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

        string GetOrderByPart(VariationReportType reportType)
        {
            string orderByExpression = String.Format("ORDER BY SUM({0}) DESC", GetSumColumnName(reportType));
            switch (reportType)
            {
                case VariationReportType.InBoundAmount:
                case VariationReportType.OutBoundAmount:
                case VariationReportType.TopDestinationAmount:
                    orderByExpression = String.Format("ORDER BY SUM((SaleNets / ISNULL(SER.Rate, 1)) - (CostNets / ISNULL(CER.Rate, 1))) DESC");
                    break;
            }
            return orderByExpression;
        }

        #endregion

        #endregion

        #region Time Periods

        DataTable BuildTimePeriodsTable(DataRetrievalInput<VariationReportQuery> input)
        {
            DataTable table = new DataTable();

            table.Columns.Add("PeriodIndex", typeof(int));
            table.Columns.Add("FromDate", typeof(DateTime));
            table.Columns.Add("ToDate", typeof(DateTime));

            table.BeginLoadData();

            DateTime toDate = input.Query.ToDate.AddDays(1); // Add 1 day to include the ToDate in the result

            for (int i = 0; i < input.Query.NumberOfPeriods; i++)
            {
                DataRow row = table.NewRow();
                DateTime fromDate = GetFromDate(input.Query.TimePeriod, toDate);

                row["PeriodIndex"] = i + 1;
                row["FromDate"] = fromDate.Date;
                row["ToDate"] = toDate.Date;

                toDate = fromDate;
                table.Rows.Add(row);
            }

            table.EndLoadData();
            return table;
        }

        DateTime GetFromDate(VariationReportTimePeriod timePeriodType, DateTime toDate)
        {
            switch (timePeriodType)
            {
                case VariationReportTimePeriod.Daily:
                    return toDate.AddDays(-1);
                case VariationReportTimePeriod.Weekly:
                    return toDate.AddDays(-7);
                case VariationReportTimePeriod.Monthly:
                    return toDate.AddDays(-30);
            }
            throw new ArgumentException("timePeriodType");
        }

        List<TimePeriod> GetTimePeriodDefinitions(DataRetrievalInput<VariationReportQuery> input)
        {
            DataTable timePeriodsTable = BuildTimePeriodsTable(input);

            var timePeriodDefinitions = new List<TimePeriod>();

            foreach (DataRow row in timePeriodsTable.Rows)
            {
                DateTime fromDate = Convert.ToDateTime(row["FromDate"]);
                DateTime toDate = Convert.ToDateTime(row["ToDate"]);

                timePeriodDefinitions.Add(new TimePeriod()
                {
                    PeriodDescription = (input.Query.TimePeriod == VariationReportTimePeriod.Daily) ?
                        fromDate.ToShortDateString() : String.Format("{0} - {1}", fromDate.ToShortDateString(), toDate.AddDays(-1).ToShortDateString()),
                    From = fromDate,
                    To = toDate
                });
            }

            return timePeriodDefinitions;
        }

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
