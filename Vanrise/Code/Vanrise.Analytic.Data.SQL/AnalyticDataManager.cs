using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Data.SQL;
using Vanrise.GenericData.Data.SQL;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Data.SQL
{
    public class AnalyticDataManager : BaseSQLDataManager, IAnalyticDataManager
    {
        #region Public Methods
        public IEnumerable<DBAnalyticRecord> GetAnalyticRecords(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input, out  HashSet<string> includedSQLDimensions)
        {
            //Dictionary<string, Object> parameterValues = new Dictionary<string, object>();
            //string query = BuildAnalyticQuery(input, false, parameterValues);


            //List<AnalyticRecord> records = GetItemsText(query, (reader) => AnalyticRecordMapper(reader, input.Query, false), (cmd) =>
            //{
            //    foreach (var prm in parameterValues)
            //    {
            //        cmd.Parameters.Add(new SqlParameter(prm.Key, prm.Value));
            //    }
            //    if (input.Query.CurrencyId.HasValue)
            //        cmd.Parameters.Add(new SqlParameter("@Currency", input.Query.CurrencyId.Value));
            //});
            HashSet<string> includedSQLDimensions_Local;
            HashSet<string> includedSQLAggregates;
            Dictionary<string, Object> parameterValues = new Dictionary<string, object>();
            string query = BuildAnalyticQuery(input, out includedSQLDimensions_Local, out includedSQLAggregates, parameterValues);
            List<DBAnalyticRecord> dbRecords = GetItemsText(query, (reader) => SQLRecordMapper(reader, null, includedSQLDimensions_Local, includedSQLAggregates), (cmd) =>
            {
                foreach (var prm in parameterValues)
                {
                    cmd.Parameters.Add(new SqlParameter(prm.Key, prm.Value));
                }
                if (input.Query.CurrencyId.HasValue)
                    cmd.Parameters.Add(new SqlParameter("@Currency", input.Query.CurrencyId.Value));
            });
            includedSQLDimensions = includedSQLDimensions_Local;
            return dbRecords;
            //AnalyticRecord summaryRecord;
            //return ProcessSQLRecords(input.Query.DimensionFields, input.Query.ParentDimensions, input.Query.MeasureFields, input.Query.Filters, input.Query.FilterGroup, sqlRecords, includedSQLDimensions, input.Query.WithSummary, out summaryRecord);
        }

        public IEnumerable<TimeVariationAnalyticRecord> GetTimeVariationAnalyticRecords(Vanrise.Entities.DataRetrievalInput<TimeVariationAnalyticQuery> input)
        {
            Dictionary<string, Object> parameterValues = new Dictionary<string, object>();
            string query = BuildTimeVariationAnalyticQuery(input, false, parameterValues);


            return GetItemsText(query, (reader) => TimeVariationAnalyticRecordMapper(reader, input.Query, false, input.Query.TimeGroupingUnit), (cmd) =>
            {
                foreach (var prm in parameterValues)
                {
                    cmd.Parameters.Add(new SqlParameter(prm.Key, prm.Value));
                }
                if (input.Query.CurrencyId.HasValue)
                    cmd.Parameters.Add(new SqlParameter("@Currency", input.Query.CurrencyId.Value));
            });
        }

        public IEnumerable<AnalyticRecord> GetTimeVariationAnalyticRecords2(Vanrise.Entities.DataRetrievalInput<TimeVariationAnalyticQuery> input)
        {
            List<DBAnalyticRecord> sqlRecords = null;
            HashSet<string> availableDimensions = null;
            AnalyticRecord summaryRecord;
            return ProcessSQLRecords(null, input.Query.ParentDimensions, input.Query.MeasureFields, input.Query.Filters, input.Query.FilterGroup, sqlRecords, availableDimensions, input.Query.WithSummary, out summaryRecord);
        }

        public AnalyticRecord GetAnalyticSummary(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input)
        {
            //Dictionary<string, Object> parameterValues = new Dictionary<string, object>();
            //string query = BuildAnalyticQuery(input, input.Query.WithSummary, parameterValues);
            //return GetItemText(query, reader => AnalyticRecordMapper(reader, input.Query, true), (cmd) =>
            //{
            //    foreach (var prm in parameterValues)
            //    {
            //        cmd.Parameters.Add(new SqlParameter(prm.Key, prm.Value));
            //    }
            //    if (input.Query.CurrencyId.HasValue)
            //        cmd.Parameters.Add(new SqlParameter("@Currency", input.Query.CurrencyId.Value));
            //});
            List<DBAnalyticRecord> sqlRecords = null;
            HashSet<string> availableDimensions = null;
            AnalyticRecord summaryRecord;
            ProcessSQLRecords(null, input.Query.ParentDimensions, input.Query.MeasureFields, input.Query.Filters, input.Query.FilterGroup, sqlRecords, availableDimensions, true, out summaryRecord);
            return summaryRecord;
        }

        #endregion

        #region Private Methods

        #region Query Builder

        string BuildAnalyticQuery(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input, out HashSet<string> includedSQLDimensionNames, out HashSet<string> includedSQLAggregateNames, Dictionary<string, Object> parameterValues)
        {
            StringBuilder selectPartBuilder = new StringBuilder();
            HashSet<string> includeJoinConfigNames = new HashSet<string>();
            includedSQLDimensionNames = GetIncludedSQLDimensionNames(input.Query.DimensionFields, input.Query.Filters, input.Query.FilterGroup);
            int parameterIndex = 0;
            string groupByPart = null;
            groupByPart = BuildQueryGrouping(selectPartBuilder, includedSQLDimensionNames, includeJoinConfigNames);
            includedSQLAggregateNames = GetIncludedSQLAggregateNames(input.Query.MeasureFields);
            BuildQueryAggregates(selectPartBuilder, includedSQLAggregateNames, includeJoinConfigNames);
            string filterPart = BuildQueryFilter(input.Query.Filters, includeJoinConfigNames);
            string joinPart = BuildQueryJoins(includeJoinConfigNames);

            //string orderPart = null;
            //if (input.Query.OrderBy != null)
            //    orderPart = BuildQueryOrder(input.Query.OrderBy);

            RecordFilterSQLBuilder recordFilterSQLBuilder = new RecordFilterSQLBuilder(GetDimensionColumnIdFromFieldName);
            String recordFilter = recordFilterSQLBuilder.BuildRecordFilter(input.Query.FilterGroup, ref parameterIndex, parameterValues);


            StringBuilder queryBuilder = BuildGlobalQuery(input.Query.FromTime, input.Query.ToTime,
                parameterValues, selectPartBuilder.ToString(), joinPart, filterPart, groupByPart, recordFilter, ref parameterIndex);

            return queryBuilder.ToString();
        }

        private HashSet<string> GetIncludedSQLDimensionNames(List<string> requestedDimensionNames, List<DimensionFilter> dimensionFilters, RecordFilterGroup filterGroup)
        {
            HashSet<string> sqlDimensions = new HashSet<string>();
            if (requestedDimensionNames != null)
            {
                foreach (var dimensionName in requestedDimensionNames)
                {
                    AddSQLDimensions(dimensionName, sqlDimensions);
                }
            }
            if (dimensionFilters != null)
            {
                foreach (var dimensionFilter in dimensionFilters)
                {
                    AddSQLDimensions(dimensionFilter.Dimension, sqlDimensions);
                }
            }
            //TODO: add dimensions from FilterGroup
            return sqlDimensions;
        }

        private void AddSQLDimensions(string dimensionName, HashSet<string> sqlDimensionNames)
        {
            var dimensionConfig = GetDimensionConfig(dimensionName);
            if (!String.IsNullOrEmpty(dimensionConfig.Config.SQLExpression))
                sqlDimensionNames.Add(dimensionConfig.Name);
            if (dimensionConfig.Config.DependentDimensions != null)
            {
                foreach (var dependentDimensionName in dimensionConfig.Config.DependentDimensions)
                {
                    if (!sqlDimensionNames.Contains(dependentDimensionName))
                    {
                        AddSQLDimensions(dependentDimensionName, sqlDimensionNames);
                    }
                }
            }
        }

        private HashSet<string> GetIncludedSQLAggregateNames(List<string> measureNames)
        {
            HashSet<string> aggregateNames = new HashSet<string>();
            foreach (var measureName in measureNames)
            {
                var measureConfig = GetMeasureConfig(measureName);
                if (measureConfig.Config.DependentAggregateNames != null)
                {
                    foreach (var aggName in measureConfig.Config.DependentAggregateNames)
                    {
                        aggregateNames.Add(aggName);
                    }
                }
            }
            return aggregateNames;
        }

        string BuildTimeVariationAnalyticQuery(Vanrise.Entities.DataRetrievalInput<TimeVariationAnalyticQuery> input, bool isSummary, Dictionary<string, object> parameterValues)
        {
            StringBuilder selectPartBuilder = new StringBuilder();
            HashSet<string> includeJoinConfigNames = new HashSet<string>();

            int parameterIndex = 0;
            string groupByPart = null;
            if (!isSummary)
                groupByPart = BuildQueryGrouping(selectPartBuilder, input.Query.TimeGroupingUnit);
            string filterPart = BuildQueryFilter(input.Query.Filters, includeJoinConfigNames);
            BuildQueryMeasures(selectPartBuilder, input.Query.ParentDimensions, input.Query.MeasureFields, includeJoinConfigNames);
            string joinPart = BuildQueryJoins(includeJoinConfigNames);
            RecordFilterSQLBuilder recordFilterSQLBuilder = new RecordFilterSQLBuilder(GetDimensionColumnIdFromFieldName);
            String recordFilter = recordFilterSQLBuilder.BuildRecordFilter(input.Query.FilterGroup, ref parameterIndex, parameterValues);

            StringBuilder queryBuilder = BuildGlobalQuery(input.Query.FromTime, input.Query.ToTime, null,
                parameterValues, selectPartBuilder.ToString(), joinPart, filterPart, groupByPart, null, recordFilter, ref parameterIndex);

            return queryBuilder.ToString();
        }

        private StringBuilder BuildGlobalQuery(DateTime fromTime, DateTime toTime, Dictionary<string, Object> parameterValues, string selectPartBuilder, string joinPartBuilder, string filterPartBuilder, string groupByPartBuilder, string recordFilter, ref int parameterIndex)
        {
            List<TimeRangeTableName> timePeriodTableNames = GetTimeRangeTableNames(fromTime, toTime);


            StringBuilder queryBuilder = new StringBuilder(@"SELECT #SELECTPART# FROM
                                                                #QUERYBODY#
                                                                #JOINPART#
			                                                    #GROUPBYPART#
                                                                ");

            string filterPart = filterPartBuilder.ToString();

            if (timePeriodTableNames == null || timePeriodTableNames.Count == 0)
                throw new NullReferenceException("timePeriodTableNames");
            if (timePeriodTableNames.Count == 1)
            {
                queryBuilder.Replace("#QUERYBODY#", GetSingleTableQueryBody(timePeriodTableNames[0], filterPart, joinPartBuilder.ToString(), ref parameterIndex, parameterValues));
                queryBuilder.Replace("#JOINPART#", "");
            }
            else
            {
                StringBuilder tableUnionBuilder = new StringBuilder();
                foreach (var timePeriodTable in timePeriodTableNames)
                {
                    if (tableUnionBuilder.Length > 0)
                        tableUnionBuilder.AppendLine(" UNION ALL ");
                    tableUnionBuilder.AppendLine(String.Format(" SELECT * FROM {0} ", GetSingleTableQueryBody(timePeriodTable, filterPart, "", ref parameterIndex, parameterValues)));
                }
                queryBuilder.Replace("#QUERYBODY#", String.Format("({0}) ant", tableUnionBuilder));
                queryBuilder.Replace("#JOINPART#", joinPartBuilder.ToString());
            }
            queryBuilder.Replace("#SELECTPART#", selectPartBuilder.ToString());
            if (!string.IsNullOrEmpty(groupByPartBuilder) && groupByPartBuilder.Length > 0)
                queryBuilder.Replace("#GROUPBYPART#", "GROUP BY " + groupByPartBuilder);
            else
                queryBuilder.Replace("#GROUPBYPART#", "");

            return queryBuilder;
        }

        private StringBuilder BuildGlobalQuery(DateTime fromTime, DateTime toTime, int? topRecords, Dictionary<string, Object> parameterValues, string selectPartBuilder, string joinPartBuilder, string filterPartBuilder, string groupByPartBuilder, string orderByPart, string recordFilter, ref int parameterIndex)
        {
            List<TimeRangeTableName> timePeriodTableNames = GetTimeRangeTableNames(fromTime, toTime);


            StringBuilder queryBuilder = new StringBuilder(@"SELECT #TOPRECORDS# #SELECTPART# FROM
                                                                #QUERYBODY#
                                                                #JOINPART#
                                                                 #WHEREPART#
			                                                    #GROUPBYPART#
                                                                #ORDERPART#");

            string filterPart = filterPartBuilder.ToString();

            queryBuilder.Replace("#WHEREPART#", recordFilter != null ? ("and " + recordFilter) : "");
            if (timePeriodTableNames == null || timePeriodTableNames.Count == 0)
                throw new NullReferenceException("timePeriodTableNames");
            if (timePeriodTableNames.Count == 1)
            {
                queryBuilder.Replace("#QUERYBODY#", GetSingleTableQueryBody(timePeriodTableNames[0], filterPart, joinPartBuilder.ToString(), ref parameterIndex, parameterValues));
                queryBuilder.Replace("#JOINPART#", "");
            }
            else
            {
                StringBuilder tableUnionBuilder = new StringBuilder();
                foreach (var timePeriodTable in timePeriodTableNames)
                {
                    if (tableUnionBuilder.Length > 0)
                        tableUnionBuilder.AppendLine(" UNION ALL ");
                    tableUnionBuilder.AppendLine(String.Format(" SELECT * FROM {0} ", GetSingleTableQueryBody(timePeriodTable, filterPart, "", ref parameterIndex, parameterValues)));
                }
                queryBuilder.Replace("#QUERYBODY#", String.Format("({0}) ant", tableUnionBuilder));
                queryBuilder.Replace("#JOINPART#", joinPartBuilder.ToString());
            }
            queryBuilder.Replace("#TOPRECORDS#", string.Format("{0}", topRecords.HasValue ? "TOP(" + topRecords.Value + ") " : ""));
            queryBuilder.Replace("#SELECTPART#", selectPartBuilder.ToString());
            if (!string.IsNullOrEmpty(groupByPartBuilder) && groupByPartBuilder.Length > 0)
                queryBuilder.Replace("#GROUPBYPART#", "GROUP BY " + groupByPartBuilder);
            else
                queryBuilder.Replace("#GROUPBYPART#", "");
            if (!string.IsNullOrEmpty(orderByPart) && orderByPart.Length > 0)
                queryBuilder.Replace("#ORDERPART#", "ORDER BY " + orderByPart);
            else
                queryBuilder.Replace("#ORDERPART#", "");
            return queryBuilder;
        }

        string GetSingleTableQueryBody(TimeRangeTableName timeRangeTableName, string filterPart, string joinPart, ref int parameterIndex, Dictionary<string, Object> parameterValues)
        {
            StringBuilder singleTableQueryBodyBuilder = new StringBuilder(@" #TABLENAME# ant WITH(NOLOCK)  
                                                                              #JOINPART#                                                                
			                                                                WHERE
			                                                               (#TIMECOLUMNNAME# >= #FromTime#  AND  (#TIMECOLUMNNAME# <= #ToTime# or #ToTime# IS NULL))
                                                                            #FILTERPART#");
            singleTableQueryBodyBuilder.Replace("#TIMECOLUMNNAME#", GetTable().Settings.TimeColumnName);
            singleTableQueryBodyBuilder.Replace("#TABLENAME#", timeRangeTableName.TableName);
            singleTableQueryBodyBuilder.Replace("#FILTERPART#", filterPart);
            singleTableQueryBodyBuilder.Replace("#JOINPART#", joinPart);
            string fromTimePrm = GenerateParameterName(ref parameterIndex);
            singleTableQueryBodyBuilder.Replace("#FromTime#", fromTimePrm);
            parameterValues.Add(fromTimePrm, timeRangeTableName.FromTime);

            string toTimePrm = GenerateParameterName(ref parameterIndex);
            singleTableQueryBodyBuilder.Replace("#ToTime#", toTimePrm);
            parameterValues.Add(toTimePrm, timeRangeTableName.ToTime);
            return singleTableQueryBodyBuilder.ToString();
        }

        private string BuildQueryGrouping(StringBuilder selectPartBuilder, IEnumerable<string> sqlDimensionNames, HashSet<string> includeJoinConfigNames)
        {
            StringBuilder groupByPartBuilder = new StringBuilder();
            foreach (string dimensionName in sqlDimensionNames)
            {
                var dimensionConfig = GetDimensionConfig(dimensionName);
                if (String.IsNullOrEmpty(dimensionConfig.Config.SQLExpression))
                    throw new NullReferenceException(String.Format("dimensionConfig.Config.SQLExpression. Dim Name '{0}'", dimensionName));

                if (dimensionConfig.Config.JoinConfigNames != null)
                {
                    foreach (var join in dimensionConfig.Config.JoinConfigNames)
                    {
                        includeJoinConfigNames.Add(join);
                    }
                }
                AddColumnToStringBuilder(groupByPartBuilder, dimensionConfig.Config.SQLExpression);
                AddColumnToStringBuilder(selectPartBuilder, String.Format("{0} AS {1}", dimensionConfig.Config.SQLExpression, GetDimensionIdColumnAlias(dimensionConfig)));
            }
            return groupByPartBuilder.ToString();
        }

        private string BuildQueryGrouping(StringBuilder selectPartBuilder, TimeGroupingUnit timeGroupingUnit)
        {
            StringBuilder queryGrouping = new StringBuilder();
            switch (timeGroupingUnit)
            {
                case TimeGroupingUnit.Hour: selectPartBuilder.AppendFormat(" CONVERT(varchar(13), {0}, 121) as [Date]", GetTable().Settings.TimeColumnName); queryGrouping.AppendFormat(" CONVERT(varchar(13), {0}, 121)", GetTable().Settings.TimeColumnName); break;
                case TimeGroupingUnit.Day: selectPartBuilder.AppendFormat(" CONVERT(varchar(10), {0}, 121)  as [Date]", GetTable().Settings.TimeColumnName); queryGrouping.AppendFormat(" CONVERT(varchar(10), {0}, 121)", GetTable().Settings.TimeColumnName); break;
            }
            return queryGrouping.ToString();
        }

        private void BuildQueryAggregates(StringBuilder selectPartBuilder, IEnumerable<string> aggregateNames, HashSet<string> includeJoinConfigNames)
        {
            foreach (var aggName in aggregateNames)
            {
                var aggregateConfig = GetAggregateConfig(aggName);

                AddColumnToStringBuilder(selectPartBuilder, String.Format("{0}({1}) AS {2}", aggregateConfig.Config.AggregateType, aggregateConfig.Config.SQLColumn, GetAggregateColumnAlias(aggregateConfig)));

                if (aggregateConfig.Config.JoinConfigNames != null)
                {
                    foreach (var join in aggregateConfig.Config.JoinConfigNames)
                    {
                        includeJoinConfigNames.Add(join);
                    }
                }
            }
        }

        private void BuildQueryMeasures(StringBuilder selectPartBuilder, List<string> allIncludedDimensions, List<string> measureFields, HashSet<string> includeJoinConfigNames)
        {
            Func<AnalyticMeasure, IGetMeasureExpressionContext, string> getMeasureExpression = (measure, getMeasureExpressionContext) =>
            {
                throw new NotImplementedException();
                // return !string.IsNullOrWhiteSpace(measure.Config.SQLExpression) ? measure.Config.SQLExpression : measure.Evaluator.GetMeasureExpression(getMeasureExpressionContext);
            };


            Func<string, IGetMeasureExpressionContext, string> getMeasureExpressionByMeasureName = (measureName, getMeasureExpressionContext) =>
            {
                AnalyticMeasure measure = GetMeasureConfig(measureName);
                return getMeasureExpression(measure, getMeasureExpressionContext);
            };

            Func<string, bool> isGroupingDimensionIncluded = (dimensionName) => allIncludedDimensions != null && allIncludedDimensions.Contains(dimensionName);

            HashSet<string> addedMeasureColumns = new HashSet<string>();
            foreach (var measureName in measureFields)
            {
                AnalyticMeasure measure = GetMeasureConfig(measureName);

                GetMeasureExpressionContext getMeasureExpressionContext = new GetMeasureExpressionContext(getMeasureExpressionByMeasureName, isGroupingDimensionIncluded);
                string measureExpression = getMeasureExpressionByMeasureName(measureName, getMeasureExpressionContext);
                AddColumnToStringBuilder(selectPartBuilder, String.Format("{0} AS {1}", measureExpression, GetMeasureColumnAlias(measure)));

                if (measure.Config.JoinConfigNames != null)
                {
                    foreach (var join in measure.Config.JoinConfigNames)
                    {
                        includeJoinConfigNames.Add(join);
                    }
                }
            }
        }

        private string BuildQueryFilter(List<DimensionFilter> filters, HashSet<string> includeJoinConfigNames)
        {
            StringBuilder filterPartBuilder = new StringBuilder();
            if (filters != null)
            {
                foreach (DimensionFilter dimensionFilter in filters)
                {
                    var dimensionConfig = GetDimensionConfig(dimensionFilter.Dimension);
                    if (String.IsNullOrEmpty(dimensionConfig.Config.SQLExpression))
                        continue;
                    AddFilterToFilterPart(filterPartBuilder, dimensionFilter.FilterValues, dimensionConfig.Config.SQLExpression);

                    if (dimensionConfig.Config.JoinConfigNames != null)
                    {
                        foreach (var join in dimensionConfig.Config.JoinConfigNames)
                        {
                            includeJoinConfigNames.Add(join);
                        }
                    }
                }
            }
            return filterPartBuilder.ToString();
        }

        private string BuildQueryJoins(HashSet<string> includeJoinConfigNames)
        {
            StringBuilder joinPartBuilder = new StringBuilder();
            foreach (string joinName in includeJoinConfigNames)
            {
                AnalyticJoin join = GetJoinConfig(joinName);
                AddStatementToJoinPart(joinPartBuilder, join.Config.JoinStatement);
            }
            return joinPartBuilder.ToString();
        }

        private string BuildQueryOrder(List<string> measureFields)
        {
            StringBuilder orderPart = new StringBuilder();
            foreach (var measureName in measureFields)
            {
                AnalyticMeasure measure = GetMeasureConfig(measureName);
                AddColumnToStringBuilder(orderPart, String.Format("{0}", GetMeasureColumnAlias(measure)));
            }
            orderPart.Append(String.Format(" desc"));
            return orderPart.ToString();
        }

        #region Query Building Helpers

        string GenerateParameterName(ref int parameterIndex)
        {
            return String.Format("@Prm_{0}", parameterIndex++);
        }

        string GetDimensionIdColumnAlias(AnalyticDimension dimension)
        {
            return String.Format("Dimension_{0}_Id", dimension.AnalyticDimensionConfigId);
        }

        string GetDimensionNameColumnAlias(AnalyticDimension dimension)
        {
            return String.Format("Dimension_{0}_Name", dimension.AnalyticDimensionConfigId);
        }
        string GetAggregateColumnAlias(AnalyticAggregate aggregateConfig)
        {
            return String.Format("Aggregate_{0}", aggregateConfig.AnalyticAggregateConfigId);
        }
        string GetMeasureColumnAlias(AnalyticMeasure measure)
        {
            return String.Format("Measure_{0}", measure.AnalyticMeasureConfigId);
        }

        string GetDimensionColumnIdFromFieldName(string fieldName)
        {
            var dimensionObj = GetDimensionConfig(fieldName);
            if (dimensionObj == null)
                throw new NullReferenceException(String.Format("Dimension {0}", fieldName));
            return dimensionObj.Config.SQLExpression;
        }

        void AddFilterToFilterPart<T>(StringBuilder filterBuilder, List<T> values, string column)
        {
            if (values != null && values.Count() > 0)
            {
                if (values[0].GetType() == typeof(string) || values[0].GetType() == typeof(DateTime))
                {
                    StringBuilder builder = new StringBuilder();
                    if (values.Count == 1)
                        builder.Append("'").Append(values[0]).Append("'");
                    else
                    {
                        foreach (T val in values)
                        {
                            builder.Append("'").Append(val).Append("' ,");
                        }
                        builder.Length--;
                    }

                    filterBuilder.AppendFormat(" AND {0} IN ({1}) ", column, builder);
                }
                else
                    filterBuilder.AppendFormat(" AND {0} IN ({1}) ", column, String.Join(", ", values));
            }
        }

        void AddColumnToStringBuilder(StringBuilder builder, string column)
        {
            if (builder.Length > 0)
            {
                builder.Append(" , ");
            }
            builder.Append(column);
        }

        void AddStatementToJoinPart(StringBuilder joinPartBuilder, string statement)
        {
            joinPartBuilder.Append(" " + statement + " ");
        }

        #endregion

        #endregion

        #region Process Final Result

        private List<AnalyticRecord> ProcessSQLRecords(List<string> requestedDimensionNames, List<string> parentDimensionNames, List<string> measureNames, List<DimensionFilter> dimensionFiltes,
            RecordFilterGroup filterGroup, List<DBAnalyticRecord> sqlRecords, HashSet<string> availableDimensions, bool withSummary, out AnalyticRecord summaryRecord)
        {
            List<string> allDimensionNamesList = new List<string>();
            if (requestedDimensionNames != null)
                allDimensionNamesList.AddRange(requestedDimensionNames);
            if (parentDimensionNames != null)
                allDimensionNamesList.AddRange(parentDimensionNames);
            HashSet<string> allDimensionNames = new HashSet<string>(allDimensionNamesList);
            FillCalculatedDimensions(requestedDimensionNames, sqlRecords, availableDimensions);
            ApplyDimensionFilter(dimensionFiltes, filterGroup, sqlRecords);
            List<AnalyticRecord> records = ApplyFinalGrouping(requestedDimensionNames, allDimensionNames, measureNames, sqlRecords, withSummary, out summaryRecord);
            return records;
        }

        private void FillCalculatedDimensions(List<string> requestedDimensionNames, List<DBAnalyticRecord> sqlRecords, HashSet<string> availableDimensions)
        {
            IEnumerable<AnalyticDimension> dimensionsToCalculate = requestedDimensionNames.Where(dimName => !availableDimensions.Contains(dimName)).Select(dimName => GetDimensionConfig(dimName));
            foreach (var sqlRecord in sqlRecords)
            {
                foreach (var dimToCalculate in dimensionsToCalculate)
                {
                    var getDimensionValueContext = new GetDimensionValueContext(sqlRecord);
                    sqlRecord.GroupingValuesByDimensionName.Add(dimToCalculate.Name, new DBAnalyticRecordGroupingValue
                    {
                        Value = dimToCalculate.Evaluator.GetDimensionValue(getDimensionValueContext)
                    });
                }
            }
        }

        private void ApplyDimensionFilter(List<DimensionFilter> dimensionFiltes, RecordFilterGroup filterGroup, List<DBAnalyticRecord> sqlRecords)
        {
            //throw new NotImplementedException();
        }

        private List<AnalyticRecord> ApplyFinalGrouping(List<string> requestedDimensionNames, HashSet<string> allDimensionNames, List<string> measureNames, List<DBAnalyticRecord> sqlRecords, bool withSummary, out AnalyticRecord summaryRecord)
        {
            Dictionary<string, DBAnalyticRecord> groupedRecordsByDimensionsKey = new Dictionary<string, DBAnalyticRecord>();
            DBAnalyticRecord summarySQLRecord = new DBAnalyticRecord() { AggValuesByAggName = new Dictionary<string, DBAnalyticRecordAggValue>() };
            foreach (var sqlRecord in sqlRecords)
            {
                string groupingKey = GetDimensionGroupingKey(requestedDimensionNames, sqlRecord);
                DBAnalyticRecord matchRecord;
                if (!groupedRecordsByDimensionsKey.TryGetValue(groupingKey, out matchRecord))
                {
                    groupedRecordsByDimensionsKey.Add(groupingKey, sqlRecord);
                }
                else
                {
                    UpdateAggregateValues(matchRecord, sqlRecord);
                }
                if (withSummary)
                    UpdateAggregateValues(summarySQLRecord, sqlRecord);
            }
            List<AnalyticRecord> analyticRecords = new List<AnalyticRecord>();
            foreach (var sqlRecord in groupedRecordsByDimensionsKey.Values)
            {
                AnalyticRecord analyticRecord = BuildAnalyticRecordFromSQLRecord(sqlRecord, requestedDimensionNames, allDimensionNames, measureNames);
                analyticRecords.Add(analyticRecord);
            }
            if (withSummary)
                summaryRecord = BuildAnalyticRecordFromSQLRecord(summarySQLRecord, null, allDimensionNames, measureNames);
            else
                summaryRecord = null;
            return analyticRecords;
        }

        private string GetDimensionGroupingKey(List<string> requestedDimensionNames, DBAnalyticRecord record)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var dimensionName in requestedDimensionNames)
            {
                DBAnalyticRecordGroupingValue groupingValue;
                if (record.GroupingValuesByDimensionName.TryGetValue(dimensionName, out groupingValue))
                {
                    builder.AppendFormat("^*^{0}", groupingValue.Value != null ? groupingValue.Value : "");
                }
                else
                    throw new NullReferenceException(String.Format("groupingValue. dimName '{0}'", dimensionName));
            }
            return builder.ToString();
        }

        private void UpdateAggregateValues(DBAnalyticRecord existingRecord, DBAnalyticRecord record)
        {
            foreach (var aggEntry in record.AggValuesByAggName)
            {
                var existingAgg = record.AggValuesByAggName[aggEntry.Key];
                var agg = aggEntry.Value;
                switch (GetAggregateConfig(aggEntry.Key).Config.AggregateType)
                {
                    case AnalyticAggregateType.Count:
                    case AnalyticAggregateType.Sum:
                        existingAgg.Value = existingAgg.Value + agg.Value;
                        break;
                    case AnalyticAggregateType.Max:
                        if (existingAgg.Value < agg.Value)
                            existingAgg.Value = agg.Value;
                        break;
                    case AnalyticAggregateType.Min:
                        if (existingAgg.Value > agg.Value)
                            existingAgg.Value = agg.Value;
                        break;
                }
            }
        }

        private AnalyticRecord BuildAnalyticRecordFromSQLRecord(DBAnalyticRecord sqlRecord, List<string> dimensionNames, HashSet<string> allDimensionNames, List<string> measureNames)
        {
            AnalyticRecord analyticRecord = new AnalyticRecord() { Time = sqlRecord.Time, DimensionValues = new DimensionValue[dimensionNames.Count], MeasureValues = new MeasureValues() };

            if (dimensionNames != null)
            {
                int dimIndex = 0;
                foreach (string dimName in dimensionNames)
                {
                    var dimensionValue = new DimensionValue();
                    dimensionValue.Value = sqlRecord.GroupingValuesByDimensionName[dimName].Value;
                    if (dimensionValue.Value != null && dimensionValue.Value != DBNull.Value)
                        dimensionValue.Name = GetDimensionConfig(dimName).Config.FieldType.GetDescription(dimensionValue.Value);
                    analyticRecord.DimensionValues[dimIndex] = dimensionValue;
                    dimIndex++;
                }
            }
            foreach (var measureName in measureNames)
            {
                var measureConfig = GetMeasureConfig(measureName);
                var getMeasureValueContext = new GetMeasureValueContext(sqlRecord, allDimensionNames);
                var measureValue = measureConfig.Evaluator.GetMeasureValue(getMeasureValueContext);
                analyticRecord.MeasureValues.Add(measureName, measureValue);
            }
            return analyticRecord;
        }

        #endregion

        #region Table Partition Query Methods

        private List<TimeRangeTableName> GetTimeRangeTableNames(DateTime fromTime, DateTime toTime)
        {
            List<TimeRangeTableName> tableNames = new List<TimeRangeTableName>();

            var timeRange = new TimeRange
            {
                FromTime = fromTime,
                ToTime = toTime
            };

            var noTimeAllocationContext = new TimeRangeTableAllocationContext
            {
                TableName = this.GetTable().Settings.TableName,
                GetPeriodStart = (tr) => tr.FromTime,
                RangeMinLength = TimeSpan.FromSeconds(0),
                IncrementPeriod = (d, tr) => d.Add((tr.ToTime - d))
            };
            var hourlyAllocationContext = new TimeRangeTableAllocationContext
            {
                TableName = GetTable().Settings.HourlyTableName,
                GetPeriodStart = GetStartOfHour,
                RangeMinLength = TimeSpan.FromHours(1),
                IncrementPeriod = (d, tr) => d.AddHours(1),
                NextTimeRangeAllocationContext = noTimeAllocationContext
            };
            var dailyAllocationContext = new TimeRangeTableAllocationContext
            {
                TableName = GetTable().Settings.DailyTableName,
                GetPeriodStart = GetStartOfDay,
                RangeMinLength = TimeSpan.FromDays(1),
                IncrementPeriod = (d, tr) => d.AddDays(1),
                NextTimeRangeAllocationContext = hourlyAllocationContext
            };
            var weeklyAllocationContext = new TimeRangeTableAllocationContext
            {
                TableName = GetTable().Settings.WeeklyTableName,
                GetPeriodStart = GetStartOfWeek,
                RangeMinLength = TimeSpan.FromDays(7),
                IncrementPeriod = (d, tr) => d.AddDays(7),
                NextTimeRangeAllocationContext = dailyAllocationContext
            };
            var monthlyAllocationContext = new TimeRangeTableAllocationContext
            {
                TableName = GetTable().Settings.MonthlyTableName,
                GetPeriodStart = GetStartOfMonth,
                RangeMinLength = TimeSpan.FromDays(28),
                IncrementPeriod = (d, tr) => d.AddMonths(1),
                NextTimeRangeAllocationContext = weeklyAllocationContext
            };

            AllocatePeriodTables(timeRange, monthlyAllocationContext, tableNames);

            return tableNames;
        }

        DateTime GetStartOfHour(TimeRange timeRange)
        {
            DateTime startTime = new DateTime(timeRange.FromTime.Year, timeRange.FromTime.Month, timeRange.FromTime.Day, timeRange.FromTime.Hour, 0, 0);
            return startTime < timeRange.FromTime ? startTime.AddHours(1) : startTime;
        }

        DateTime GetStartOfDay(TimeRange timeRange)
        {
            DateTime startTime = new DateTime(timeRange.FromTime.Year, timeRange.FromTime.Month, timeRange.FromTime.Day, 0, 0, 0);
            return startTime < timeRange.FromTime ? startTime.AddDays(1) : startTime;
        }

        DateTime GetStartOfWeek(TimeRange timeRange)
        {
            DateTime startTime = new DateTime(timeRange.FromTime.Year, 1, 1);
            for (var dt = startTime; dt < timeRange.ToTime; dt = dt.AddDays(7))
            {
                if (dt >= timeRange.FromTime)
                    return dt;
            }
            return timeRange.ToTime;
        }

        DateTime GetStartOfMonth(TimeRange timeRange)
        {
            DateTime startTime = new DateTime(timeRange.FromTime.Year, timeRange.FromTime.Month, 1, 0, 0, 0);
            return startTime < timeRange.FromTime ? startTime.AddMonths(1) : startTime;
        }

        void AllocatePeriodTables(TimeRange timeRange, TimeRangeTableAllocationContext context, List<TimeRangeTableName> tableNames)
        {
            List<TimeRange> notAllocatedRanges;
            AllocatePeriodTables(timeRange, context.TableName, context.GetPeriodStart, context.RangeMinLength, context.IncrementPeriod, tableNames, out notAllocatedRanges);
            if (context.NextTimeRangeAllocationContext != null && notAllocatedRanges != null && notAllocatedRanges.Count > 0)
            {
                foreach (var subRange in notAllocatedRanges)
                {
                    AllocatePeriodTables(subRange, context.NextTimeRangeAllocationContext, tableNames);
                }
            }
        }

        void AllocatePeriodTables(TimeRange timeRange, string tableName, Func<TimeRange, DateTime> getPeriodStart, TimeSpan rangeMinLength, Func<DateTime, TimeRange, DateTime> incrementPeriod, List<TimeRangeTableName> tableNames, out List<TimeRange> notAllocatedRanges)
        {
            notAllocatedRanges = new List<TimeRange>();
            if (!String.IsNullOrEmpty(tableName) && (timeRange.ToTime - timeRange.FromTime) >= rangeMinLength)
            {
                DateTime startTime = getPeriodStart(timeRange);
                DateTime? allocatedRangeStart = null;
                DateTime? allocatedRangeEnd = null;
                for (DateTime dt = startTime; dt < timeRange.ToTime; dt = incrementPeriod(dt, timeRange))
                {
                    DateTime rangeEnd = incrementPeriod(dt, timeRange);
                    if (rangeEnd <= timeRange.ToTime)
                    {
                        if (dt == startTime)//first range
                            allocatedRangeStart = dt;
                        allocatedRangeEnd = rangeEnd;
                    }
                }

                if (allocatedRangeStart.HasValue)
                {
                    tableNames.Add(new TimeRangeTableName
                    {
                        TableName = tableName,
                        FromTime = allocatedRangeStart.Value,
                        ToTime = allocatedRangeEnd.Value
                    });
                    if (allocatedRangeStart.Value > timeRange.FromTime)
                        notAllocatedRanges.Add(new TimeRange { FromTime = timeRange.FromTime, ToTime = allocatedRangeStart.Value });
                    if (allocatedRangeEnd.Value < timeRange.ToTime)
                        notAllocatedRanges.Add(new TimeRange { FromTime = allocatedRangeEnd.Value, ToTime = timeRange.ToTime });
                }
                else
                    notAllocatedRanges.Add(timeRange);
            }
            else
                notAllocatedRanges.Add(timeRange);
        }

        #endregion

        #region Mappers


        private DBAnalyticRecord SQLRecordMapper(IDataReader reader, TimeGroupingUnit? timeGroupingUnit, HashSet<string> includedSQLDimensions, HashSet<string> includedSQLAggregates)
        {
            DBAnalyticRecord record = new DBAnalyticRecord() { GroupingValuesByDimensionName = new Dictionary<string, DBAnalyticRecordGroupingValue>(), AggValuesByAggName = new Dictionary<string, DBAnalyticRecordAggValue>() };
            if (timeGroupingUnit.HasValue)
            {
                string dateTime = reader["Date"] as string;
                DateTime dateTimeParsedValue = new DateTime();
                switch (timeGroupingUnit)
                {
                    case TimeGroupingUnit.Day: DateTime.TryParseExact(dateTime, "yyyy-MM-dd", CultureInfo.CurrentCulture, DateTimeStyles.None, out dateTimeParsedValue); break;
                    case TimeGroupingUnit.Hour: DateTime.TryParseExact(dateTime, "yyyy-MM-dd HH", CultureInfo.CurrentCulture, DateTimeStyles.None, out dateTimeParsedValue); break;
                }
                if (dateTimeParsedValue != default(DateTime))
                    record.Time = dateTimeParsedValue;
            }
            record.GroupingValuesByDimensionName = new Dictionary<string, DBAnalyticRecordGroupingValue>();

            if (includedSQLDimensions != null)
            {
                foreach (var dimensionName in includedSQLDimensions)
                {
                    var dimensionConfig = GetDimensionConfig(dimensionName);
                    var dimensionValue = reader[GetDimensionIdColumnAlias(dimensionConfig)];
                    record.GroupingValuesByDimensionName.Add(dimensionName, new DBAnalyticRecordGroupingValue { Value = dimensionValue != DBNull.Value ? dimensionValue : null });
                }
            }

            if (includedSQLAggregates != null)
            {
                foreach (var aggregateName in includedSQLAggregates)
                {
                    var aggregateConfig = GetAggregateConfig(aggregateName);
                    var aggregateValue = reader[GetAggregateColumnAlias(aggregateConfig)];
                    record.AggValuesByAggName.Add(aggregateName, new DBAnalyticRecordAggValue { Value = aggregateValue != DBNull.Value ? aggregateValue : null });
                }
            }

            return record;
        }

        AnalyticRecord AnalyticRecordMapper(System.Data.IDataReader reader, AnalyticQuery analyticQuery, bool isSummary)
        {
            AnalyticRecord record = new AnalyticRecord()
                {
                    MeasureValues = new MeasureValues()
                };
            var index = 0;
            if (!isSummary)
            {
                record.DimensionValues = analyticQuery.DimensionFields != null ? new DimensionValue[analyticQuery.DimensionFields.Count] : new DimensionValue[0];

                if (analyticQuery.DimensionFields != null)
                {
                    foreach (var dimensionName in analyticQuery.DimensionFields)
                    {
                        var dimension = GetDimensionConfig(dimensionName);
                        object dimensionId = GetReaderValue<object>(reader, GetDimensionIdColumnAlias(dimension));
                        string name = "";
                        if (dimensionId != null)
                        {

                            if (string.IsNullOrEmpty(dimension.Config.NameColumn))
                                name = dimension.Config.FieldType.GetDescription(dimensionId);
                            else
                                name = reader[GetDimensionNameColumnAlias(dimension)] as string;
                        }

                        record.DimensionValues[index] = new DimensionValue
                        {
                            Value = dimensionId != null ? dimensionId : "",
                            Name = name
                        };


                        index++;
                    }
                }
            }
            record.MeasureValues = MeasureValuesMapper(reader, analyticQuery.MeasureFields);
            return record;
        }

        TimeVariationAnalyticRecord TimeVariationAnalyticRecordMapper(IDataReader reader, TimeVariationAnalyticQuery timeVariationAnalyticQuery, bool isSummary, TimeGroupingUnit timeGroupingUnit)
        {
            TimeVariationAnalyticRecord record = new TimeVariationAnalyticRecord();
            string dateTime = reader["Date"] as string;
            DateTime dateTimeParsedValue = new DateTime();
            switch (timeGroupingUnit)
            {
                case TimeGroupingUnit.Day: DateTime.TryParseExact(dateTime, "yyyy-MM-dd", CultureInfo.CurrentCulture, DateTimeStyles.None, out dateTimeParsedValue); break;
                case TimeGroupingUnit.Hour: DateTime.TryParseExact(dateTime, "yyyy-MM-dd HH", CultureInfo.CurrentCulture, DateTimeStyles.None, out dateTimeParsedValue); break;
            }
            if (dateTimeParsedValue != default(DateTime))
                record.Time = dateTimeParsedValue;
            record.MeasureValues = MeasureValuesMapper(reader, timeVariationAnalyticQuery.MeasureFields);
            return record;
        }

        private MeasureValues MeasureValuesMapper(System.Data.IDataReader reader, List<string> measureFields)
        {
            MeasureValues measureValues = new MeasureValues();
            foreach (var measureName in measureFields)
            {
                var measure = GetMeasureConfig(measureName);
                measureValues.Add(measureName, GetReaderValue<Object>(reader, GetMeasureColumnAlias(measure)));
            }
            return measureValues;
        }

        #endregion

        #endregion

        #region IAnalyticDataManager Implementation

        IAnalyticTableQueryContext _analyticTableQueryContext;
        public IAnalyticTableQueryContext AnalyticTableQueryContext
        {
            set { _analyticTableQueryContext = value; }
        }

        AnalyticTable GetTable()
        {
            if (_analyticTableQueryContext == null)
                throw new NullReferenceException("_analyticTableQueryContext");
            return _analyticTableQueryContext.GetTable();
        }

        AnalyticDimension GetDimensionConfig(string dimensionName)
        {
            if (_analyticTableQueryContext == null)
                throw new NullReferenceException("_analyticTableQueryContext");
            return _analyticTableQueryContext.GetDimensionConfig(dimensionName);
        }

        AnalyticAggregate GetAggregateConfig(string aggregateName)
        {
            if (_analyticTableQueryContext == null)
                throw new NullReferenceException("_analyticTableQueryContext");
            return _analyticTableQueryContext.GetAggregateConfig(aggregateName);
        }

        AnalyticMeasure GetMeasureConfig(string measureName)
        {
            if (_analyticTableQueryContext == null)
                throw new NullReferenceException("_analyticTableQueryContext");
            return _analyticTableQueryContext.GetMeasureConfig(measureName);
        }

        AnalyticJoin GetJoinConfig(string joinName)
        {
            if (_analyticTableQueryContext == null)
                throw new NullReferenceException("_analyticTableQueryContext");
            return _analyticTableQueryContext.GetJoinContig(joinName);
        }

        #endregion

        #region Overriden Methods
        protected override string GetConnectionString()
        {
            return GetTable().Settings.ConnectionString;
        }

        #endregion

        #region Private Classes

        public class TimeRange
        {
            public DateTime FromTime { get; set; }

            public DateTime ToTime { get; set; }
        }

        private class TimeRangeTableName : TimeRange
        {
            public string TableName { get; set; }
        }

        public class TimeRangeTableAllocationContext
        {
            public string TableName { get; set; }
            public Func<TimeRange, DateTime> GetPeriodStart { get; set; }
            public TimeSpan RangeMinLength { get; set; }
            public Func<DateTime, TimeRange, DateTime> IncrementPeriod { get; set; }
            public TimeRangeTableAllocationContext NextTimeRangeAllocationContext { get; set; }
        }

        #endregion
    }
}
