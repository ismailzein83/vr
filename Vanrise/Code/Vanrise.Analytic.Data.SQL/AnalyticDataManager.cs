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
    public class AnalyticDataManager : BaseSQLDataManager, Entities.IAnalyticDataManager
    {
        #region Public Methods
        public AnalyticDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }

        public IEnumerable<DBAnalyticRecord> GetAnalyticRecords(AnalyticQuery query, out  HashSet<string> includedSQLDimensions)
        {            
            HashSet<string> includedSQLDimensions_Local;
            HashSet<string> includedSQLAggregates;
            Dictionary<string, Object> parameterValues = new Dictionary<string, object>();
            string querySQL = BuildAnalyticQuery(query, out includedSQLDimensions_Local, out includedSQLAggregates, parameterValues);
            
            if(parameterValues.Count > 0)
            {
                StringBuilder parameterDeclarationBuilder = new StringBuilder();
                StringBuilder parameterAssignementBuilder = new StringBuilder();
                foreach(var prm in parameterValues)
                {
                    if (parameterDeclarationBuilder.Length > 0)
                        parameterDeclarationBuilder.Append(", ");
                    if (parameterAssignementBuilder.Length > 0)
                        parameterAssignementBuilder.Append(", ");
                    
                    parameterDeclarationBuilder.Append(String.Format("{0} {1}", prm.Key, GetSqlParameterSqlType(prm.Key, prm.Value)));
                    parameterAssignementBuilder.Append(String.Format("{0} = {0}_FromOut", prm.Key));
                }
                querySQL = string.Format(@"DECLARE {0}  
                SELECT {1}
                   {2} ", parameterDeclarationBuilder, parameterAssignementBuilder, querySQL);
            }
            if (query.CurrencyId.HasValue)
            {
                querySQL = string.Format(@"DECLARE @Currency int
                SELECT @Currency = @Currency_FromOut
                {0}", querySQL);
            }
            List<DBAnalyticRecord> dbRecords = GetItemsText(querySQL, (reader) => SQLRecordMapper(reader, query.TimeGroupingUnit, includedSQLDimensions_Local, includedSQLAggregates), (cmd) =>
            {
                foreach (var prm in parameterValues)
                {
                    cmd.Parameters.Add(new SqlParameter(String.Format("{0}_FromOut", prm.Key), prm.Value));
                }
                if (query.CurrencyId.HasValue)
                    cmd.Parameters.Add(new SqlParameter("@Currency_FromOut", query.CurrencyId.Value));
            });
            includedSQLDimensions = includedSQLDimensions_Local;
            return dbRecords;
        }
                
        #endregion

        #region Private Methods

        #region Query Builder

        string BuildAnalyticQuery(AnalyticQuery query, out HashSet<string> includedSQLDimensionNames, out HashSet<string> includedSQLAggregateNames, Dictionary<string, Object> parameterValues)
        {
            StringBuilder selectPartBuilder = new StringBuilder();
            HashSet<string> includeJoinConfigNames = new HashSet<string>();
            includedSQLDimensionNames = GetIncludedSQLDimensionNames(query.DimensionFields, query.MeasureFields, query.Filters, query.FilterGroup);
            int parameterIndex = 0;
            string groupByPart = null;
            HashSet<string> joinStatements = new HashSet<string>();
            var toTime = query.ToTime.HasValue ? query.ToTime.Value : DateTime.Now;
            List<string> listCurrencySQLColumnNames = new List<string>();
            groupByPart = BuildQueryGrouping(selectPartBuilder, query.CurrencyId, listCurrencySQLColumnNames, includedSQLDimensionNames, query.TimeGroupingUnit, includeJoinConfigNames, joinStatements, query.FromTime, toTime, parameterValues, ref parameterIndex);
            includedSQLAggregateNames = GetIncludedSQLAggregateNames(query.MeasureFields);
            BuildQueryAggregates(selectPartBuilder, query.CurrencyId, listCurrencySQLColumnNames, includedSQLAggregateNames, includeJoinConfigNames, joinStatements, query.FromTime, toTime, parameterValues, ref parameterIndex);
            string filterPart = BuildQueryFilter(query.Filters, includeJoinConfigNames, parameterValues, ref parameterIndex);
            string joinPart = BuildQueryJoins(includeJoinConfigNames, joinStatements);

            RecordFilterSQLBuilder recordFilterSQLBuilder = new RecordFilterSQLBuilder(GetDimensionColumnIdFromFieldName);
            String recordFilter = recordFilterSQLBuilder.BuildRecordFilter(query.FilterGroup, ref parameterIndex, parameterValues);


            StringBuilder queryBuilder = BuildGlobalQuery(query.FromTime, toTime,
                parameterValues, selectPartBuilder.ToString(), joinPart, filterPart, groupByPart, recordFilter, ref parameterIndex);

            return queryBuilder.ToString();
        }

        //string BuildTimeVariationAnalyticQuery(Vanrise.Entities.DataRetrievalInput<TimeVariationAnalyticQuery> input, out HashSet<string> includedSQLDimensionNames, out HashSet<string> includedSQLAggregateNames, Dictionary<string, object> parameterValues)
        //{
        //    StringBuilder selectPartBuilder = new StringBuilder();
        //    HashSet<string> includeJoinConfigNames = new HashSet<string>();

        //    int parameterIndex = 0;
        //    string groupByPart = null;
        //    if (!isSummary)
        //        groupByPart = BuildQueryGrouping(selectPartBuilder, input.Query.TimeGroupingUnit);
        //    string filterPart = BuildQueryFilter(input.Query.Filters, includeJoinConfigNames);
        //    BuildQueryMeasures(selectPartBuilder, input.Query.ParentDimensions, input.Query.MeasureFields, includeJoinConfigNames);
        //    string joinPart = BuildQueryJoins(includeJoinConfigNames);
        //    RecordFilterSQLBuilder recordFilterSQLBuilder = new RecordFilterSQLBuilder(GetDimensionColumnIdFromFieldName);
        //    String recordFilter = recordFilterSQLBuilder.BuildRecordFilter(input.Query.FilterGroup, ref parameterIndex, parameterValues);

        //    StringBuilder queryBuilder = BuildGlobalQuery(input.Query.FromTime, input.Query.ToTime, null,
        //        parameterValues, selectPartBuilder.ToString(), joinPart, filterPart, groupByPart, null, recordFilter, ref parameterIndex);

        //    return queryBuilder.ToString();
        //}

        private HashSet<string> GetIncludedSQLDimensionNames(List<string> requestedDimensionNames, List<string> measureNames, List<DimensionFilter> dimensionFilters, RecordFilterGroup filterGroup)
        {
            HashSet<string> sqlDimensions = new HashSet<string>();
            if (requestedDimensionNames != null)
            {
                foreach (var dimensionName in requestedDimensionNames)
                {
                    AddSQLDimensions(dimensionName, sqlDimensions);
                }
            }
            if(measureNames != null)
            {
                foreach(var measureName in measureNames)
                {
                    var measureConfig = GetMeasureConfig(measureName);
                    if(measureConfig.Config.DependentDimensions != null)
                    {
                        foreach(var measureDepDim in measureConfig.Config.DependentDimensions)
                        {
                            AddSQLDimensions(measureDepDim, sqlDimensions);
                        }
                    }
                }
            }
            var filterDimensions = GetDimensionNamesFromQueryFilters();
            if (filterDimensions != null)
            {
                foreach (var filterDimension in filterDimensions)
                {
                    AddSQLDimensions(filterDimension, sqlDimensions);
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

        string GetSingleTableQueryBody(TimeRangeTableName timeRangeTableName, string filterPart, string joinPart, ref int parameterIndex, Dictionary<string, Object> parameterValues)
        {
            StringBuilder singleTableQueryBodyBuilder = new StringBuilder(@" #TABLENAME# ant WITH(NOLOCK)  
                                                                              #JOINPART#                                                                
			                                                                WHERE
			                                                               (#TIMECOLUMNNAME# >= #FromTime#  AND #TIMECOLUMNNAME# <= #ToTime#)
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

        private string BuildQueryGrouping(StringBuilder selectPartBuilder, int? requestedCurrencyId, List<string> listCurrencySQLColumnNames, IEnumerable<string> sqlDimensionNames, TimeGroupingUnit? timeGroupingUnit, HashSet<string> includeJoinConfigNames, HashSet<string> joinStatements, DateTime fromTime, DateTime toTime, Dictionary<string, Object> parameterValues, ref int parameterIndex)
        {
            StringBuilder groupByPartBuilder = new StringBuilder();
            if (sqlDimensionNames != null)
            {
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
                    string columnSQLExpression;
                    if (!String.IsNullOrEmpty(dimensionConfig.Config.CurrencySQLColumnName))
                    {
                        string currencyConversionStatement = GetCurrencyConversionStatement(requestedCurrencyId, listCurrencySQLColumnNames, dimensionConfig.Config.CurrencySQLColumnName, joinStatements, fromTime, toTime, parameterValues, ref parameterIndex);
                        columnSQLExpression = String.Format("CONVERT(DECIMAL(20, 8), {0}{1})", dimensionConfig.Config.SQLExpression, currencyConversionStatement);
                    }
                    else
                    {
                        columnSQLExpression = dimensionConfig.Config.SQLExpression;
                    }
                    AddColumnToStringBuilder(groupByPartBuilder, columnSQLExpression);
                    AddColumnToStringBuilder(selectPartBuilder, String.Format("{0} AS {1}", columnSQLExpression, GetDimensionIdColumnAlias(dimensionConfig)));
                }
            }
            if(timeGroupingUnit.HasValue)
            {
                string timeSQLFormula = null;
                switch (timeGroupingUnit)
                {
                    case TimeGroupingUnit.Hour: timeSQLFormula = String.Format(" CONVERT(varchar(13), {0}, 121)", GetTable().Settings.TimeColumnName);break;
                    case TimeGroupingUnit.Day: timeSQLFormula = String.Format(" CONVERT(varchar(10), {0}, 121)", GetTable().Settings.TimeColumnName); break;
                }
                AddColumnToStringBuilder(groupByPartBuilder, timeSQLFormula);
                AddColumnToStringBuilder(selectPartBuilder, String.Format("{0} AS [Date]", timeSQLFormula));
            }

            return groupByPartBuilder.ToString();
        }

        private void BuildQueryAggregates(StringBuilder selectPartBuilder, int? requestedCurrencyId, List<string> listCurrencySQLColumnNames, IEnumerable<string> aggregateNames, HashSet<string> includeJoinConfigNames, HashSet<string> joinStatements, DateTime fromTime, DateTime toTime, Dictionary<string, Object> parameterValues, ref int parameterIndex)
        {
            foreach (var aggName in aggregateNames)
            {
                var aggregateConfig = GetAggregateConfig(aggName);

                if (!String.IsNullOrEmpty(aggregateConfig.Config.CurrencySQLColumnName))
                {
                    string currencyConversionStatement = GetCurrencyConversionStatement(requestedCurrencyId, listCurrencySQLColumnNames, aggregateConfig.Config.CurrencySQLColumnName, joinStatements, fromTime, toTime, parameterValues, ref parameterIndex);
                    AddColumnToStringBuilder(selectPartBuilder, String.Format("CONVERT(DECIMAL(20, 8), {0}({1}{2})) AS {3}", aggregateConfig.Config.AggregateType, aggregateConfig.Config.SQLColumn, currencyConversionStatement, GetAggregateColumnAlias(aggregateConfig)));
                }
                else
                {
                    AddColumnToStringBuilder(selectPartBuilder, String.Format("{0}({1}) AS {2}", aggregateConfig.Config.AggregateType, aggregateConfig.Config.SQLColumn, GetAggregateColumnAlias(aggregateConfig)));
                }

                if (aggregateConfig.Config.JoinConfigNames != null)
                {
                    foreach (var join in aggregateConfig.Config.JoinConfigNames)
                    {
                        includeJoinConfigNames.Add(join);
                    }
                }
            }
        }

        private string GetCurrencyConversionStatement(int? requestedCurrencyId, List<string> listCurrencySQLColumnNames, string currencySQLColumnName, HashSet<string> joinStatements, DateTime fromTime, DateTime toTime, Dictionary<string, Object> parameterValues, ref int parameterIndex)
        {
            string currencySQLColumnNameLower = currencySQLColumnName.Trim().ToLower();
            string currencyTableAlias = String.Format("CurrExch_{0}", currencySQLColumnNameLower);
            if (!listCurrencySQLColumnNames.Contains(currencySQLColumnNameLower))
            {
                string currencyTableStatement;
                string fromTimePrm = GenerateParameterName(ref parameterIndex);
                parameterValues.Add(fromTimePrm, fromTime);
                string toTimePrm = GenerateParameterName(ref parameterIndex);
                parameterValues.Add(toTimePrm, toTime);
                if (requestedCurrencyId.HasValue)
                {
                    string currencyIdPrm = GenerateParameterName(ref parameterIndex);
                    parameterValues.Add(currencyIdPrm, requestedCurrencyId.Value);
                    currencyTableStatement = String.Format("(select * from Common.getExchangeRatesConvertedToCurrency({0} , {1}, {2}))", currencyIdPrm, fromTimePrm, toTimePrm);
                }
                else
                    currencyTableStatement = String.Format("(select * from Common.getExchangeRates({0} , {1}))", fromTimePrm, toTimePrm);
                joinStatements.Add(String.Format(@"LEFT JOIN {0} AS {1} ON ant.{2} = {1}.CurrencyID AND ant.{3} >= {1}.BED AND ({1}.EED IS NULL OR ant.{3} < {1}.EED)"
                                , currencyTableStatement, currencyTableAlias, currencySQLColumnName, GetTable().Settings.TimeColumnName));

                listCurrencySQLColumnNames.Add(currencySQLColumnNameLower);
            }

            return String.Format("/ ISNULL({0}.Rate, 1)", currencyTableAlias);
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

        private string BuildQueryFilter(List<DimensionFilter> filters, HashSet<string> includeJoinConfigNames, Dictionary<string, Object> parameterValues, ref int parameterIndex)
        {
            StringBuilder filterPartBuilder = new StringBuilder();
            if (filters != null)
            {
                foreach (DimensionFilter dimensionFilter in filters)
                {
                    var dimensionConfig = GetDimensionConfig(dimensionFilter.Dimension);
                    if (String.IsNullOrEmpty(dimensionConfig.Config.SQLExpression))
                        continue;
                    AddFilterToFilterPart(filterPartBuilder, dimensionFilter.FilterValues, dimensionConfig.Config.SQLExpression, parameterValues, ref parameterIndex);

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

        private string BuildQueryJoins(HashSet<string> includeJoinConfigNames, HashSet<string> joinStatements)
        {
            StringBuilder joinPartBuilder = new StringBuilder();
            foreach (string joinName in includeJoinConfigNames)
            {
                AnalyticJoin join = GetJoinConfig(joinName);
                AddStatementToJoinPart(joinPartBuilder, join.Config.JoinStatement);
            }
            foreach(var joinStatement in joinStatements)
            {
                AddStatementToJoinPart(joinPartBuilder, joinStatement);
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
            return String.Format("Dimension_{0}_Id", dimension.AnalyticDimensionConfigId.ToString().Replace("-", ""));
        }

        string GetDimensionNameColumnAlias(AnalyticDimension dimension)
        {
            return String.Format("Dimension_{0}_Name", dimension.AnalyticDimensionConfigId.ToString().Replace("-", ""));
        }
        string GetAggregateColumnAlias(AnalyticAggregate aggregateConfig)
        {
            return String.Format("Aggregate_{0}", aggregateConfig.AnalyticAggregateConfigId.ToString().Replace("-", ""));
        }
        string GetMeasureColumnAlias(AnalyticMeasure measure)
        {
            return String.Format("Measure_{0}", measure.AnalyticMeasureConfigId.ToString().Replace("-", ""));
        }

        string GetDimensionColumnIdFromFieldName(string fieldName)
        {
            var dimensionObj = GetDimensionConfig(fieldName);
            if (dimensionObj == null)
                throw new NullReferenceException(String.Format("Dimension {0}", fieldName));
            return dimensionObj.Config.SQLExpression;
        }

        void AddFilterToFilterPart<T>(StringBuilder filterBuilder, List<T> values, string column, Dictionary<string, Object> parameterValues, ref int parameterIndex)
        {
            if (values != null && values.Count() > 0)
            {                
                bool hasNullValue = false;
                List<string> parameterNames = new List<string>();
                foreach(var value in values)
                {
                    if (value == null)
                        hasNullValue = true;
                    else
                    {
                        string prmName = GenerateParameterName(ref parameterIndex);
                        parameterValues.Add(prmName, value);
                        parameterNames.Add(prmName);
                    }
                }
                List<string> filters = new List<string>();                
                if(parameterNames.Count > 0)
                    filters.Add( String.Format(" {0} IN ({1}) ", column, String.Join(", ", parameterNames)));
                if (hasNullValue)
                    filters.Add(string.Format(" {0} IS NULL ", column));
                
                filterBuilder.AppendFormat(" AND ({0}) ", String.Join(" OR ", filters));
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
                IncrementPeriod = (d, tr) => d.Add((tr.ToTime - d))
            };
            var hourlyAllocationContext = new TimeRangeTableAllocationContext
            {
                TableName = GetTable().Settings.HourlyTableName,
                GetPeriodStart = GetStartOfHour,
                IncrementPeriod = (d, tr) => d.AddHours(1),
                NextTimeRangeAllocationContext = noTimeAllocationContext
            };
            var dailyAllocationContext = new TimeRangeTableAllocationContext
            {
                TableName = GetTable().Settings.DailyTableName,
                GetPeriodStart = GetStartOfDay,
                IncrementPeriod = (d, tr) => d.AddDays(1),
                NextTimeRangeAllocationContext = hourlyAllocationContext
            };
            var weeklyAllocationContext = new TimeRangeTableAllocationContext
            {
                TableName = GetTable().Settings.WeeklyTableName,
                GetPeriodStart = GetStartOfWeek,
                IncrementPeriod = (d, tr) => d.AddDays(7).Year == d.Year ? d.AddDays(7) : new DateTime(d.Year + 1, 1, 1),
                NextTimeRangeAllocationContext = dailyAllocationContext
            };
            var monthlyAllocationContext = new TimeRangeTableAllocationContext
            {
                TableName = GetTable().Settings.MonthlyTableName,
                GetPeriodStart = GetStartOfMonth,
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
            AllocatePeriodTables(timeRange, context.TableName, context.GetPeriodStart, context.IncrementPeriod, tableNames, out notAllocatedRanges);
            if (context.NextTimeRangeAllocationContext != null && notAllocatedRanges != null && notAllocatedRanges.Count > 0)
            {
                foreach (var subRange in notAllocatedRanges)
                {
                    AllocatePeriodTables(subRange, context.NextTimeRangeAllocationContext, tableNames);
                }
            }
        }

        void AllocatePeriodTables(TimeRange timeRange, string tableName, Func<TimeRange, DateTime> getPeriodStart, Func<DateTime, TimeRange, DateTime> incrementPeriod, List<TimeRangeTableName> tableNames, out List<TimeRange> notAllocatedRanges)
        {
            notAllocatedRanges = new List<TimeRange>();
            if (!String.IsNullOrEmpty(tableName))
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
                measureValues.Add(measureName, new MeasureValue { Value = GetReaderValue<Object>(reader, GetMeasureColumnAlias(measure)) });
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

        List<string> GetDimensionNamesFromQueryFilters()
        {
            if (_analyticTableQueryContext == null)
                throw new NullReferenceException("_analyticTableQueryContext");
            return _analyticTableQueryContext.GetDimensionNamesFromQueryFilters();
        }

        #endregion

        #region Overriden Methods
        protected override string GetConnectionString()
        {
            var tableSettings = GetTable().Settings;
            return !String.IsNullOrEmpty(tableSettings.ConnectionString) ? tableSettings.ConnectionString : Common.Utilities.GetExposedConnectionString(tableSettings.ConnectionStringName);
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
            public Func<DateTime, TimeRange, DateTime> IncrementPeriod { get; set; }
            public TimeRangeTableAllocationContext NextTimeRangeAllocationContext { get; set; }
        }

        #endregion
    }
}
