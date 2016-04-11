﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Data.SQL;

namespace Vanrise.Analytic.Data.SQL
{
    public class AnalyticDataManager : BaseSQLDataManager, IAnalyticDataManager
    {
        static string statTableName = "TOneWhS_Analytic.TrafficStats";
        public AnalyticSummaryBigResult<AnalyticRecord> GetAnalyticRecords(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input)
        {
            AnalyticConfigurationDataManager dataManager = new AnalyticConfigurationDataManager();
            var dimensionsConfig = dataManager.GetFilteredDimensions(input.Query.DimensionFields);
            var measureConfig = dataManager.GetFilteredMeasures(input.Query.MeasureFields);
            var dimensionsFilterConfig = dataManager.GetFilteredDimensions(input.Query.DimensionFields);
            string tempTable = null;

            Action<string> createTempTableIfNotExistsAction = (tempTableName) =>
            {
                tempTable = tempTableName;
                string query = BuildAnalyticSummaryQuery(input, tempTableName, dimensionsConfig, measureConfig, dimensionsFilterConfig);

                if (input.Query.Currency != null)
                    ExecuteNonQueryText(query, (cmd) =>
                    {
                        cmd.Parameters.Add(new SqlParameter("@FromDate", input.Query.FromTime));
                        cmd.Parameters.Add(new SqlParameter("@ToDate", ToDBNullIfDefault(input.Query.ToTime)));
                        cmd.Parameters.Add(new SqlParameter("@Currency", input.Query.Currency));
                    });
                else
                    ExecuteNonQueryText(query, (cmd) =>
                    {
                        cmd.Parameters.Add(new SqlParameter("@FromDate", input.Query.FromTime));
                        cmd.Parameters.Add(new SqlParameter("@ToDate", ToDBNullIfDefault(input.Query.ToTime)));
                    });
            };

            Dictionary<string, string> columnsMappings = new Dictionary<string, string>();
            for (int i = 0; i < input.Query.DimensionFields.Count; i++)
            {
                var groupField = input.Query.DimensionFields[i];
                columnsMappings.Add(String.Format("DimensionValues[{0}].Name", i), String.Format("DimensionName_{0}", groupField));
            }

            for (int i = 0; i < input.Query.MeasureFields.Count; i++)
            {
                var measureField = input.Query.MeasureFields[i];
                var measureFieldConfig = measureConfig[measureField];
                if (measureFieldConfig.ColumnName != null)
                    columnsMappings.Add(String.Format("MeasureValues.{0}", input.Query.MeasureFields[i]), measureFieldConfig.ColumnName);
            }

            AnalyticSummaryBigResult<AnalyticRecord> rslt = RetrieveData(input, createTempTableIfNotExistsAction, (reader) => AnalyticRecordMapper(reader, dimensionsConfig, measureConfig)
            , columnsMappings, new AnalyticSummaryBigResult<AnalyticRecord>()) as AnalyticSummaryBigResult<AnalyticRecord>;

            //if (input.Query.WithSummary)
            //    rslt.Summary = GetSummary(input, tempTable, dimensionsConfig, measureFieldsConfig);
            return rslt;
        }

        private AnalyticRecord AnalyticRecordMapper(System.Data.IDataReader reader, Dictionary<string, DimensionConfiguration> dimensionsConfig, Dictionary<string, MeasureConfiguration> measureConfig)
        {
            AnalyticRecord record = new AnalyticRecord()
            {
                DimensionValues = dimensionsConfig != null ? new DimensionValue[dimensionsConfig.Count] : new DimensionValue[0],
                MeasureValues = new MeasureValues() // Object[measureFieldsConfig.Count]
            };
            var index = 0;
            if (dimensionsConfig != null)
                foreach (var groupFieldConfig in dimensionsConfig)
                {
                    object value = GetReaderValue<object>(reader, String.Format("DimensionId_{0}", groupFieldConfig.Key));
                    if (value != null)
                    {
                        string nameColumnName = String.Format("DimensionName_{0}", groupFieldConfig.Key);
                        record.DimensionValues[index] = new DimensionValue
                        {
                            Value = value,
                            Name = reader[nameColumnName].ToString()
                        };
                    }

                    index++;
                }
            index = 0;
            foreach (var measureFieldConfig in measureConfig)
            {
                //if (measureFieldConfig.Value.GetMeasureValue != null)
                //    record.MeasureValues[measureFieldConfig.Key] = measureFieldConfig.Value.GetMeasureValue(reader, record);
                index++;
            }
            index = 0;
            return record;
        }

        private string BuildAnalyticSummaryQuery(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input, string tempTableName, Dictionary<string, DimensionConfiguration> dimensionsConfig, Dictionary<string, MeasureConfiguration> measureConfig, Dictionary<string, DimensionConfiguration> dimensionsFilterConfig)
        {
            StringBuilder selectPartBuilder = new StringBuilder();
            StringBuilder joinPartBuilder = new StringBuilder();
            StringBuilder filterPartBuilder = new StringBuilder();
            StringBuilder groupByPartBuilder = new StringBuilder();
            StringBuilder ctePartBuilder = new StringBuilder();
            StringBuilder billingJoinPartBuilder = new StringBuilder();

            string tableNamePartBuilder = statTableName;
            List<string> lstCTEStatements = new List<string>();
            HashSet<string> lstJoinStatement = new HashSet<string>();
            List<string> lstWhereStatement = new List<string>();

            #region MainQuery
            StringBuilder queryBuilder = new StringBuilder(@"IF NOT OBJECT_ID('#TEMPTABLE#', N'U') IS NOT NULL
	                                                        BEGIN
                                                                ;WITH 
                                                                --#CTEPART#
                                                                AllResult AS( 
			                                                    SELECT #TOPRECORDS# #SELECTPART#
			                                                    FROM #TABLENAME# bs --WITH(NOLOCK ,INDEX(#TABLEINDEX#))
                                                                #JOINPART#
                                                                #EXCHANGEJOINPART#
			                                                    WHERE
			                                                   (FirstCDRAttempt >= @FromDate  AND  (FirstCDRAttempt <=@ToDate or @ToDate IS NULL))
                                                                #FILTERPART#
			                                                    #GROUPBYPART#)
                                                                SELECT * INTO #TEMPTABLE# FROM AllResult
                                                            END ");
            #endregion

            #region Adding Group Fields Part
            foreach (string groupField in input.Query.DimensionFields)
            {
                DimensionConfiguration groupFieldConfig = dimensionsConfig[groupField];



                if (groupFieldConfig.GroupByStatement != null)
                    AddColumnToStringBuilder(groupByPartBuilder, groupFieldConfig.GroupByStatement);

                if (groupFieldConfig.JoinStatement != null)

                    if (!lstJoinStatement.Contains(groupFieldConfig.JoinStatement))
                    {
                        lstJoinStatement.Add(groupFieldConfig.JoinStatement);
                        AddStatementToJoinPart(joinPartBuilder, groupFieldConfig.JoinStatement);
                    }



                if (!String.IsNullOrEmpty(groupFieldConfig.ColumnId))
                    AddColumnToStringBuilder(selectPartBuilder, String.Format("{0} AS DimensionId_{1}", groupFieldConfig.ColumnId, groupField));

                if (!String.IsNullOrEmpty(groupFieldConfig.ColumnName))
                    AddColumnToStringBuilder(selectPartBuilder, String.Format("{0} AS DimensionName_{1}", groupFieldConfig.ColumnName, groupField));

            }
            #endregion

            #region Adding FilterPart

            if (input.Query.Filters != null)
            {
                foreach (DimensionFilter dimensionFilter in input.Query.Filters)
                {


                    DimensionConfiguration filterFieldConfig = dimensionsFilterConfig[dimensionFilter.Dimension];

                    if (!String.IsNullOrEmpty(filterFieldConfig.ColumnId))
                        AddFilterToFilterPart(filterPartBuilder, dimensionFilter.FilterValues, filterFieldConfig.ColumnId);

                    if (dimensionsFilterConfig.ContainsKey(dimensionFilter.Dimension))
                    {
                        DimensionConfiguration groupFieldConfig = dimensionsFilterConfig[dimensionFilter.Dimension];
                        if (groupFieldConfig.JoinStatement != null)
                            if (!lstJoinStatement.Contains(groupFieldConfig.JoinStatement))
                            {
                                lstJoinStatement.Add(groupFieldConfig.JoinStatement);
                                AddStatementToJoinPart(joinPartBuilder, groupFieldConfig.JoinStatement);
                            }
                    }
                }
            }

            #endregion

            #region Add Measures Part
            HashSet<string> addedMeasureColumns = new HashSet<string>();
            foreach (var measureName in input.Query.MeasureFields)
            {
                var measure = measureConfig[measureName];
                if (!(string.IsNullOrEmpty(measure.Expression.SQLExpression) && addedMeasureColumns.Contains(measure.Expression.SQLExpression)))
                {
                    AddColumnToStringBuilder(selectPartBuilder, String.Format("{0} AS {1}", measure.Expression.SQLExpression, measure.ColumnName));
                }
            }

            #endregion

            #region Replacment Query

            queryBuilder.Replace("#TOPRECORDS#", string.Format("{0}", input.Query.TopRecords != null ? "TOP(" + input.Query.TopRecords + ") " : ""));
            queryBuilder.Replace("#TABLENAME#", tableNamePartBuilder);
            queryBuilder.Replace("#TEMPTABLE#", tempTableName);
            queryBuilder.Replace("#TABLEINDEX#", "");
            queryBuilder.Replace("#SELECTPART#", selectPartBuilder.ToString());
            queryBuilder.Replace("#JOINPART#", joinPartBuilder.ToString());
            queryBuilder.Replace("#FILTERPART#", filterPartBuilder.ToString());

            if (groupByPartBuilder.Length > 0)
                queryBuilder.Replace("#GROUPBYPART#", "GROUP BY " + groupByPartBuilder);
            else
                queryBuilder.Replace("#GROUPBYPART#", "");
            //queryBuilder.Replace("#CTEPART#", ctePartBuilder.ToString());
            //if (input.Query.Currency != null)
            //queryBuilder.Replace("#EXCHANGEJOINPART#", JoinStatement.ConvertedToCurrency);
            //else
            //queryBuilder.Replace("#EXCHANGEJOINPART#", JoinStatement.ExchangeCurrency);
            queryBuilder.Replace("#EXCHANGEJOINPART#", "");

            #endregion

            return queryBuilder.ToString();
        }

        private void AddFilterToFilterPart(StringBuilder filterPartBuilder, List<object> list, string p)
        {
            throw new NotImplementedException();
        }

        private void AddColumnToStringBuilder(StringBuilder builder, string column)
        {
            if (builder.Length > 0)
            {
                builder.Append(" , ");
            }
            builder.Append(column);
        }

        private void AddStatementToStringBuilder(StringBuilder builder, string statement)
        {
            if (statement.Length != 0)
                builder.Append(" " + statement + " , ");
        }

        private void AddStatementToJoinPart(StringBuilder joinPartBuilder, string statement)
        {
            joinPartBuilder.Append(" " + statement + " ");
        }
        private void AddPartOfJoinStatment(StringBuilder joinPartBuilder, string statement)
        {
            if (joinPartBuilder != null && joinPartBuilder.Length > 0)
                joinPartBuilder.Append("AND");
            joinPartBuilder.Append(" " + statement + " ");
        }

        public class JoinStatement
        {
            public const string ConvertedToCurrency = "LEFT JOIN ExchangeRatesConvertedToCurrency ERC ON ERC.CurrencyID = bs.CostCurrency AND ERC.BED>=bs.CallDate and( ERC.EED IS NULL OR ERC.EED <bs.CallDate) LEFT JOIN ExchangeRatesConvertedToCurrency ERS ON ERS.CurrencyID = bs.SaleCurrency AND ERS.BED>=bs.CallDate and( ERS.EED IS NULL OR ERS.EED <bs.CallDate) ";
            public const string ExchangeCurrency = "LEFT JOIN ExchangeRates ERC ON ERC.CurrencyID = bs.CostCurrency AND ERC.BED>=bs.CallDate and( ERC.EED IS NULL OR ERC.EED <bs.CallDate) LEFT JOIN ExchangeRates ERS ON ERS.CurrencyID = bs.SaleCurrency AND ERS.BED>=bs.CallDate and( ERS.EED IS NULL OR ERS.EED <bs.CallDate) ";
        }

    }
}
