﻿using System;
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
        #region Constructors

        public VariationReportDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneV2DBConnString"))
        {

        }
        
        #endregion

        #region Public Methods

        public IEnumerable<VariationReportRecord> GetFilteredVariationReportRecords(DataRetrievalInput<VariationReportQuery> input, IEnumerable<TimePeriod> timePeriods)
        {
            DataTable timePeriodsTable = GetTimePeriodsTable(timePeriods);

            return GetItemsText<VariationReportRecord>(GetVariationReportQuery(input.Query), (reader) => VariationReportRecordMapper(reader, input.Query), (cmd) =>
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

        DataTable GetTimePeriodsTable(IEnumerable<TimePeriod> timePeriods)
        {
            DataTable table = new DataTable();

            table.Columns.Add("PeriodIndex", typeof(int));
            table.Columns.Add("FromDate", typeof(DateTime));
            table.Columns.Add("ToDate", typeof(DateTime));

            table.BeginLoadData();

            for (int i = 0; i < timePeriods.Count(); i++)
            {
                DataRow row = table.NewRow();
                row["PeriodIndex"] = i + 1;
                row["FromDate"] = timePeriods.ElementAt(i).From;
                row["ToDate"] = timePeriods.ElementAt(i).To;
                table.Rows.Add(row);
            }

            table.EndLoadData();
            return table;
        }

        #region Variation Report Query

        string GetVariationReportQuery(VariationReportQuery query)
        {
            var variationReportQueryBuilder = new StringBuilder
            (@"
                #EXCHANGE_RATES_TABLE#
                #QUERY_PART#
            ");

            variationReportQueryBuilder.Replace("#EXCHANGE_RATES_TABLE#", GetExchangeRatesTable(query.CurrencyId));

            string queryPart = (query.ReportType == VariationReportType.InOutBoundMinutes || query.ReportType == VariationReportType.InOutBoundAmount) ?
                GetMultiBoundQuery(query) :
                GetSingleBoundQuery(query, VariationReportRecordDimensionSuffix.None, null);

            variationReportQueryBuilder.Replace("#QUERY_PART#", queryPart);

            return variationReportQueryBuilder.ToString();
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

        #region Multi Bound Query

        string GetMultiBoundQuery(VariationReportQuery query)
        {
            var multiBoundQueryBuilder = new StringBuilder
            (@"
                #IN_BOUND_QUERY#
                #OUT_BOUND_QUERY#

                SELECT 3.0 OrderValue,
                    InResult.DimensionId AS DimensionId,
                    #TOTAL_DIMENSION_SUFFIX# AS DimensionSuffix,
                    InResult.Average - OutResult.Average Average,
                    InResult.Percentage - OutResult.Percentage Percentage,
                    InResult.PreviousPeriodPercentage - OutResult.PreviousPeriodPercentage PreviousPeriodPercentage
                    #TOTAL_RESULT_SUM_PART#
                INTO #TotalResult
                FROM #InResult InResult JOIN #OutResult OutResult ON InResult.DimensionId = OutResult.DimensionId

                UPDATE #InResult SET OrderValue = 3.0 WHERE DimensionId IN (SELECT DimensionId FROM #TotalResult)
                UPDATE #OutResult SET OrderValue = 3.0 WHERE DimensionId IN (SELECT DimensionId FROM #TotalResult)

                SELECT *
                FROM (SELECT * FROM #InResult UNION ALL SELECT * FROM #OutResult UNION ALL SELECT * FROM #TotalResult) AS FinalResult
                ORDER BY OrderValue, DimensionId, DimensionSuffix
            ");

            VariationReportQuery inResultQuery;
            VariationReportQuery outResultQuery;
            SetInOutResultQueries(query, out inResultQuery, out outResultQuery);

            multiBoundQueryBuilder.Replace("#IN_BOUND_QUERY#", GetSingleBoundQuery(inResultQuery, VariationReportRecordDimensionSuffix.In, "#InResult"));
            multiBoundQueryBuilder.Replace("#OUT_BOUND_QUERY#", GetSingleBoundQuery(outResultQuery, VariationReportRecordDimensionSuffix.Out, "#OutResult"));
            multiBoundQueryBuilder.Replace("#TOTAL_DIMENSION_SUFFIX#", String.Format("{0}", (int)VariationReportRecordDimensionSuffix.Total));
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

            inResultQuery.ParentDimensions = query.ParentDimensions;
            inResultQuery.ToDate = query.ToDate;
            inResultQuery.TimePeriod = query.TimePeriod;
            inResultQuery.NumberOfPeriods = query.NumberOfPeriods;
            inResultQuery.GroupByProfile = query.GroupByProfile;

            outResultQuery.ParentDimensions = query.ParentDimensions;
            outResultQuery.ToDate = query.ToDate;
            outResultQuery.TimePeriod = query.TimePeriod;
            outResultQuery.NumberOfPeriods = query.NumberOfPeriods;
            outResultQuery.GroupByProfile = query.GroupByProfile;
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

        string GetSingleBoundQuery(VariationReportQuery query, VariationReportRecordDimensionSuffix dimensionSuffix, string intoTableName)
        {
            var singleResultQueryBuilder = new StringBuilder
            (@"
                SELECT #ORDER_VALUE_PART#
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
            ");

            singleResultQueryBuilder.Replace("#DIMENTION_COLUMN_NAME#", GetGroupByColumnName(query.ReportType, query.GroupByProfile));
            singleResultQueryBuilder.Replace("#DIMENSION_SUFFIX#", String.Format(", {0} AS DimensionSuffix", (int)dimensionSuffix));

            string sumArgument = GetSumArgument(query.ReportType, query.ParentDimensions);
            string sumExtension = IsDurationReport(query.ReportType) ? " / 60" : null;
            string sumAggregation = String.Format("SUM({0}){1} / {2}", sumArgument, sumExtension, query.NumberOfPeriods);

            string orderValue = GetOrderValuePart(intoTableName, query.ReportType, sumAggregation);
            singleResultQueryBuilder.Replace("#ORDER_VALUE_PART#", orderValue);

            singleResultQueryBuilder.Replace("#AVERAGE_PART#", String.Format(", {0} AS Average", sumAggregation));
            singleResultQueryBuilder.Replace("#PERCENTAGE_PART#", GetPercentagePart(sumArgument, sumExtension, sumAggregation));
            singleResultQueryBuilder.Replace("#PREVIOUS_PERIOD_PERCENTAGE_PART#", GetPreviousPeriodPercentagePart(sumArgument, sumExtension));

            singleResultQueryBuilder.Replace("#SUM_PART#", GetSumPart(query.NumberOfPeriods, sumArgument, sumExtension));
            singleResultQueryBuilder.Replace("#INTO_PART#", (intoTableName != null) ? String.Format("INTO {0}", intoTableName) : null);

            singleResultQueryBuilder.Replace("#JOIN_PART#", GetJoinPart(query));
            singleResultQueryBuilder.Replace("#WHERE_PART#", GetWherePart(query.ParentDimensions, query.GroupByProfile));

            return singleResultQueryBuilder.ToString();
        }

        string GetOrderValuePart(string intoTableName, VariationReportType reportType, string sumAggregation)
        {
            var orderValuePartBuilder = new StringBuilder();

            if (intoTableName != null) // The root report type is InOutBound
            {
                switch (reportType)
                {
                    case VariationReportType.InBoundMinutes:
                    case VariationReportType.InBoundAmount:
                        orderValuePartBuilder.Append("2.0"); // OrderValue is of type decimal
                        break;

                    case VariationReportType.OutBoundMinutes:
                    case VariationReportType.OutBoundAmount:
                        orderValuePartBuilder.Append("1.0");
                        break;
                }
                if (orderValuePartBuilder.Length == 0) // reportType MUST be either InBound/OutBound - Minutes/Amount
                    throw new ArgumentException("reportType");
            }
            else
                orderValuePartBuilder.Append(sumAggregation);


            orderValuePartBuilder.Append(" AS OrderValue,");
            return orderValuePartBuilder.ToString();
        }

        string GetGroupByColumnName(VariationReportType reportType, bool groupByProfile)
        {
            switch (reportType)
            {
                case VariationReportType.InBoundMinutes:
                case VariationReportType.InBoundAmount:
                case VariationReportType.Profit:
                    return groupByProfile ? "CustomerAccounts.CarrierProfileID" : "CustomerID";

                case VariationReportType.OutBoundMinutes:
                case VariationReportType.OutBoundAmount:
                case VariationReportType.OutBoundProfit:
                    return groupByProfile ? "SupplierAccounts.CarrierProfileID" : "SupplierID";

                case VariationReportType.TopDestinationMinutes:
                case VariationReportType.TopDestinationAmount:
                case VariationReportType.TopDestinationProfit:
                    return "SaleZoneID";

                default:
                    throw new ArgumentException("reportType");
            }
        }

        string GetPercentagePart(string sumArgument, string sumExtension, string sumAggregation)
        {
            return String.Format
            (
                ", ((SUM(CASE WHEN p1.FromDate IS NOT NULL THEN {0} ELSE 0 END){1}) - ({2})) / dbo.ZeroToMax({2}) * 100 AS Percentage",
                sumArgument,
                sumExtension,
                sumAggregation
            );
        }

        string GetPreviousPeriodPercentagePart(string sumArgument, string sumExtension)
        {
            return String.Format
            (
                @", ((SUM(CASE WHEN p1.FromDate IS NOT NULL THEN {0} ELSE 0 END){1}) - (SUM(CASE WHEN p2.FromDate IS NOT NULL THEN {0} ELSE 0 END){1})) /
                    dbo.ZeroToMax(SUM(CASE WHEN p2.FromDate IS NOT NULL THEN {0} ELSE 0 END){1}) * 100 AS PreviousPeriodPercentage",
                sumArgument,
                sumExtension
            );
        }

        string GetSumPart(int numberOfPeriods, string sumArgument, string sumExtension)
        {
            StringBuilder sumPartBuilder = new StringBuilder();
            for (int i = 0; i < numberOfPeriods; i++)
                sumPartBuilder.Append(String.Format(", SUM(CASE WHEN p{0}.FromDate IS NOT NULL THEN {1} ELSE 0 END){2} Period{0}Sum", i + 1, sumArgument, sumExtension));
            return sumPartBuilder.ToString();
        }

        #region Sum Expression

        string GetSumArgument(VariationReportType reportType, IEnumerable<ParentDimension> parentDimensions)
        {
            if (reportType == VariationReportType.Profit || reportType == VariationReportType.OutBoundProfit || reportType == VariationReportType.TopDestinationProfit)
                return "(SaleNets / ISNULL(SER.Rate, 1)) - (CostNets / ISNULL(CER.Rate, 1))";

            if (reportType == VariationReportType.InBoundMinutes)
                return "SaleDuration";
            if (reportType == VariationReportType.InBoundAmount)
                return "SaleNets / ISNULL(SER.Rate, 1)";

            if (reportType == VariationReportType.OutBoundMinutes)
                return (parentDimensions == null) ? "CostDuration" : "SaleDuration";
            if (reportType == VariationReportType.OutBoundAmount)
                return (parentDimensions == null) ? "CostNets / ISNULL(CER.Rate, 1)" : "SaleNets / ISNULL(SER.Rate, 1)";

            ParentDimension directParentDimension = (parentDimensions != null) ? parentDimensions.ElementAt(parentDimensions.Count() - 1) : null;

            if (reportType == VariationReportType.TopDestinationMinutes)
                return (directParentDimension == null || directParentDimension.Dimension == VariationReportDimension.Customer) ? "SaleDuration" : "CostDuration";
            if (reportType == VariationReportType.TopDestinationAmount)
                return (directParentDimension == null || directParentDimension.Dimension == VariationReportDimension.Customer) ?
                    "SaleNets / ISNULL(SER.Rate, 1)" : "CostNets / ISNULL(CER.Rate, 1)";

            throw new ArgumentException("reportType");
        }

        bool IsDurationReport(VariationReportType reportType)
        {
            switch (reportType)
            {
                case VariationReportType.InBoundMinutes:
                case VariationReportType.OutBoundMinutes:
                case VariationReportType.TopDestinationMinutes:
                    return true;
                case VariationReportType.InBoundAmount:
                case VariationReportType.OutBoundAmount:
                case VariationReportType.TopDestinationAmount:
                case VariationReportType.Profit:
                case VariationReportType.OutBoundProfit:
                case VariationReportType.TopDestinationProfit:
                    return false;
                default:
                    throw new ArgumentException("reportType");
            }
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
                case VariationReportType.Profit:
                case VariationReportType.OutBoundProfit:
                case VariationReportType.TopDestinationProfit:
                    joinPartBuilder.Append(String.Format(" LEFT JOIN @ConvertedExchangeRates SER on bs.SaleCurrency = SER.CurrencyID AND bs.CallDate >= SER.BED AND (SER.EED IS NULL OR bs.CallDate < SER.EED)"));
                    joinPartBuilder.Append(String.Format(" LEFT JOIN @ConvertedExchangeRates CER on bs.CostCurrency = CER.CurrencyID AND bs.CallDate >= CER.BED AND (CER.EED IS NULL OR bs.CallDate < CER.EED)"));
                    break;
            }
            for (int i = 0; i < query.NumberOfPeriods; i++)
            {
                joinPartBuilder.Append(String.Format(" LEFT JOIN @TimePeriods p{0} on bs.CallDate >= p{0}.FromDate AND bs.CallDate < p{0}.ToDate AND p{0}.PeriodIndex = {0}", i + 1));
            }
            if (query.GroupByProfile)
            {
                joinPartBuilder.Append(" INNER JOIN TOneWhS_BE.CarrierAccount CustomerAccounts ON BS.CustomerID = CustomerAccounts.ID INNER JOIN TOneWhS_BE.CarrierAccount SupplierAccounts ON BS.SupplierID = SupplierAccounts.ID");
            }
            return joinPartBuilder.ToString();
        }

        #region Where Part

        string GetWherePart(IEnumerable<ParentDimension> parentDimensions, bool groupByProfile)
        {
            var wherePartBuilder = new StringBuilder("WHERE bs.CallDate >= @SmallestFromDate AND CallDate < @LargestToDate");
            if (parentDimensions != null)
            {
                // The loop doesn't account for filter values that should be enclosed using single quotes
                foreach (ParentDimension parentDimension in parentDimensions)
                    wherePartBuilder.AppendFormat(" AND {0} IN ({1})", GetDimensionName(parentDimension.Dimension, groupByProfile), parentDimension.Value);
            }
            return wherePartBuilder.ToString();
        }

        string GetDimensionName(VariationReportDimension dimension, bool groupByProfile)
        {
            switch (dimension)
            {
                case VariationReportDimension.Customer:
                    return groupByProfile ? "CustomerAccounts.CarrierProfileID" : "CustomerID";
                case VariationReportDimension.Supplier:
                    return groupByProfile ? "SupplierAccounts.CarrierProfileID" : "SupplierID";
                case VariationReportDimension.Zone:
                    return "SaleZoneID";
            }
            throw new ArgumentException("dimension");
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

        VariationReportRecord VariationReportRecordMapper(IDataReader reader, VariationReportQuery query)
        {
            var variationReportRecord = new VariationReportRecord()
            {
                DimensionId = (object)reader["DimensionId"],
                DimensionSuffix = (VariationReportRecordDimensionSuffix)reader["DimensionSuffix"],
                OrderValue = (decimal)reader["OrderValue"],
                Average = (decimal)reader["Average"],
                Percentage = Convert.ToDecimal((double)reader["Percentage"]),
                PreviousPeriodPercentage = Convert.ToDecimal((double)reader["PreviousPeriodPercentage"])
            };

            if (query != null)
            {
                variationReportRecord.TimePeriodValues = new List<decimal>();
                for (int i = 0; i < query.NumberOfPeriods; i++)
                    variationReportRecord.TimePeriodValues.Add((decimal)reader[String.Format("Period{0}Sum", i + 1)]);
            }

            return variationReportRecord;
        }

        #endregion
    }
}

