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
            StringBuilder createTempTableQueryBuilder = new StringBuilder
            (
                @"IF NOT OBJECT_ID('#TEMP_TABLE_NAME#', N'U') IS NOT NULL
                BEGIN 
                    SELECT #TOP_PART##DIMENTION_COLUMN_NAME# AS DimensionId#DAYS_AVG_PART##DAYS_PERCENTAGE_PART##PREVIOUS_PERIOD_PERCENTAGE_PART##SUM_PART#
                    INTO #TEMP_TABLE_NAME#
                    
                    FROM TOneWhs_Analytic.BillingStats bs#JOIN_PART#
                    
                    WHERE bs.CallDate >= @SmallestFromDate AND CallDate < @LargestToDate
                    
                    GROUP BY #DIMENTION_COLUMN_NAME#
                    ORDER BY SUM(#SUM_COLUMN_NAME#) DESC
                END"
            );

            string sumColumnName = GetSumColumnName(input.Query.ReportType);

            createTempTableQueryBuilder.Replace("#TEMP_TABLE_NAME#", tempTableName);
            createTempTableQueryBuilder.Replace("#TOP_PART#", GetTopPart(input.Query.ReportType));
            createTempTableQueryBuilder.Replace("#DIMENTION_COLUMN_NAME#", GetDimentionColumnNameAndSetDimensionTitle(input.Query.ReportType));

            createTempTableQueryBuilder.Replace("#DAYS_AVG_PART#", GetDaysAveragePart(input.Query.ReportType, input.Query.NumberOfPeriods));
            createTempTableQueryBuilder.Replace("#DAYS_PERCENTAGE_PART#", GetDaysPercentagePart(input.Query.ReportType, input.Query.NumberOfPeriods));
            createTempTableQueryBuilder.Replace("#PREVIOUS_PERIOD_PERCENTAGE_PART#", GetPreviousPeriodPercentagePart(input.Query.ReportType, input.Query.NumberOfPeriods));

            createTempTableQueryBuilder.Replace("#SUM_PART#", GetSumPart(input.Query.NumberOfPeriods, sumColumnName));
            createTempTableQueryBuilder.Replace("#JOIN_PART#", GetJoinPart(input.Query.NumberOfPeriods));
            createTempTableQueryBuilder.Replace("#SUM_COLUMN_NAME#", sumColumnName);

            ExecuteNonQueryText(createTempTableQueryBuilder.ToString(), (cmd) =>
            {
                DataTable timePeriodsTable = BuildTimePeriodsTable(input);
                var tableParameter = new SqlParameter("@TimePeriods", SqlDbType.Structured);

                tableParameter.TypeName = "TOneWhS_Analytic.VariationReportTimePeriodType";
                tableParameter.Value = timePeriodsTable;
                cmd.Parameters.Add(tableParameter);

                cmd.Parameters.Add(new SqlParameter("@SmallestFromDate", GetSmallestFromDate(timePeriodsTable)));
                cmd.Parameters.Add(new SqlParameter("@LargestToDate", input.Query.ToDate));
            });
        }

        string GetTopPart(VariationReportType reportType)
        {
            return (reportType == VariationReportType.TopDestinationMinutes) ? "TOP 5 " : null;
        }

        string GetDimentionColumnNameAndSetDimensionTitle(VariationReportType reportType)
        {
            string dimensionColumnName;
            switch (reportType)
            {
                case VariationReportType.InBoundMinutes:
                    dimensionColumnName = "CustomerID";
                    _dimensionTitle = "Customer";
                    break;
                case VariationReportType.OutBoundMinutes:
                    dimensionColumnName = "SupplierID";
                    _dimensionTitle = "Supplier";
                    break;
                case VariationReportType.TopDestinationMinutes:
                    dimensionColumnName = "SaleZoneID";
                    _dimensionTitle = "Sale Zone";
                    break;
                default:
                    throw new ArgumentException("reportType");
            }
            return dimensionColumnName;
        }

        string GetDaysAveragePart(VariationReportType reportType, int numberOfPeriods)
        {
            return String.Format(", SUM({0}) / 60 / {1} AS DaysAvg", GetSumColumnName(reportType), numberOfPeriods);
        }

        string GetDaysPercentagePart(VariationReportType reportType, int numberOfPeriods)
        {
            return String.Format(", ((SUM(CASE WHEN p1.FromDate IS NOT NULL THEN {0} ELSE 0 END) / 60) - (SUM({0}) / 60 / {1})) / dbo.ZeroToMax(SUM({0}) / 60 / {1}) * 100 as DaysPercentage", GetSumColumnName(reportType), numberOfPeriods);
        }

        string GetPreviousPeriodPercentagePart(VariationReportType reportType, int numberOfPeriods)
        {
            return (numberOfPeriods > 1) ?
                String.Format(", ((SUM(CASE WHEN p1.FromDate IS NOT NULL THEN {0} ELSE 0 END) / 60) - (SUM(CASE WHEN p2.FromDate IS NOT NULL THEN {0} ELSE 0 END) / 60)) / dbo.ZeroToMax(SUM(CASE WHEN p2.FromDate IS NOT NULL THEN {0} ELSE 0 END) / 60) * 100 as PreviousPeriodPercentage", GetSumColumnName(reportType), numberOfPeriods) : null;
        }

        string GetSumPart(int rowsCount, string sumColumnName)
        {
            StringBuilder sumPartBuilder = new StringBuilder();
            for (int i = 0; i < rowsCount; i++)
            {
                sumPartBuilder.Append(String.Format(", SUM(CASE WHEN p{0}.FromDate IS NOT NULL THEN {1} ELSE 0 END) / 60 Period{0}Sum", i + 1, sumColumnName));
            }
            return sumPartBuilder.ToString();
        }

        string GetJoinPart(int rowsCount)
        {
            StringBuilder joinPartBuilder = new StringBuilder();
            for (int i = 0; i < rowsCount; i++)
            {
                joinPartBuilder.Append(String.Format(" LEFT JOIN @TimePeriods p{0} on bs.CallDate >= p{0}.FromDate AND bs.CallDate < p{0}.ToDate AND p{0}.PeriodIndex = {0}", i + 1));
            }
            return joinPartBuilder.ToString();
        }

        DateTime GetSmallestFromDate(DataTable timePeriodsTable)
        {
            DataRow lastRow = timePeriodsTable.Rows[timePeriodsTable.Rows.Count - 1];
            return Convert.ToDateTime(lastRow["FromDate"]);
        }

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
                default:
                    throw new ArgumentException("reportType");
            }
            return sumColumnName;
        }

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
                row["FromDate"] = fromDate;
                row["ToDate"] = toDate;

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

        #endregion

        #region Mappers

        VariationReportRecord VariationReportRecordMapper(IDataReader reader)
        {
            var variationReportRecord = new VariationReportRecord()
            {
                DimensionId = (object)reader["DimensionId"],
                Average = (decimal)reader["DaysAvg"],
                Percentage = Convert.ToDecimal((double)reader["DaysPercentage"]),
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
