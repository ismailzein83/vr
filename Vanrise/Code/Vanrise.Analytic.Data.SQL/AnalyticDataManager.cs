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

namespace Vanrise.Analytic.Data.SQL
{
    public class AnalyticDataManager : BaseSQLDataManager, IAnalyticDataManager
    {
        #region Public Methods
        public IEnumerable<AnalyticRecord> GetAnalyticRecords(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input)
        {
            Dictionary<string, Object> parameterValues = new Dictionary<string, object>();
            string query = BuildAnalyticQuery(input, false, parameterValues);


            return GetItemsText(query, (reader) => AnalyticRecordMapper(reader, input.Query, false), (cmd) =>
            {
                foreach (var prm in parameterValues)
                {
                    cmd.Parameters.Add(new SqlParameter(prm.Key, prm.Value));
                }
                if (input.Query.CurrencyId.HasValue)
                    cmd.Parameters.Add(new SqlParameter("@Currency", input.Query.CurrencyId.Value));
            });
        }

        public IEnumerable<TimeVariationAnalyticRecord> GetTimeVariationAnalyticRecords(Vanrise.Entities.DataRetrievalInput<TimeVariationAnalyticQuery> input)
        {
            Dictionary<string, Object> parameterValues = new Dictionary<string, object>();
            string query = BuildTimeVariationAnalyticQuery(input, false, parameterValues);


            return GetItemsText(query, (reader) => TimeVariationAnalyticRecordMapper(reader, input.Query, false,input.Query.TimeGroupingUnit), (cmd) =>
            {
                foreach (var prm in parameterValues)
                {
                    cmd.Parameters.Add(new SqlParameter(prm.Key, prm.Value));
                }
                if (input.Query.CurrencyId.HasValue)
                    cmd.Parameters.Add(new SqlParameter("@Currency", input.Query.CurrencyId.Value));
            });
        }

        public AnalyticRecord GetAnalyticSummary(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input)
        {
            return GetSummary(input);
        }

        #endregion

        #region Private Methods
        AnalyticRecord GetSummary(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input)
        {
            Dictionary<string, Object> parameterValues = new Dictionary<string, object>();
            string query = BuildAnalyticQuery(input, input.Query.WithSummary, parameterValues);
            return GetItemText(query, reader => AnalyticRecordMapper(reader, input.Query, true), (cmd) =>
                {
                    foreach(var prm in parameterValues)
                    {
                        cmd.Parameters.Add(new SqlParameter(prm.Key, prm.Value));
                    }
                    if (input.Query.CurrencyId.HasValue)
                        cmd.Parameters.Add(new SqlParameter("@Currency", input.Query.CurrencyId.Value));
                });
        }

        #region Query Builder
        string BuildAnalyticQuery(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input, bool isSummary, Dictionary<string, Object> parameterValues)
        {
            StringBuilder selectPartBuilder = new StringBuilder();
            HashSet<string> includeJoinConfigNames = new HashSet<string>();

            int parameterIndex = 0;
            string groupByPart = null;
            if (!isSummary)
                groupByPart = BuildQueryGrouping(selectPartBuilder, input.Query.DimensionFields, includeJoinConfigNames);
            string filterPart = BuildQueryFilter(input.Query.Filters, includeJoinConfigNames);
            List<string> allIncludedDimensions = new List<string>(input.Query.DimensionFields);
            if (input.Query.ParentDimensions != null)
                allIncludedDimensions.AddRange(input.Query.ParentDimensions);
            BuildQueryMeasures(selectPartBuilder, allIncludedDimensions, input.Query.MeasureFields, includeJoinConfigNames);
            string joinPart = BuildQueryJoins(includeJoinConfigNames);

            StringBuilder queryBuilder = BuildGlobalQuery(input.Query.FromTime, input.Query.ToTime, input.Query.TopRecords,
                parameterValues, selectPartBuilder.ToString(), joinPart, filterPart, groupByPart, ref parameterIndex);

            return queryBuilder.ToString();
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

            StringBuilder queryBuilder = BuildGlobalQuery(input.Query.FromTime, input.Query.ToTime, null,
                parameterValues, selectPartBuilder.ToString(), joinPart, filterPart, groupByPart, ref parameterIndex);

            return queryBuilder.ToString();
        }

        private string BuildQueryGrouping(StringBuilder selectPartBuilder, List<string> dimensionFields, HashSet<string> includeJoinConfigNames)
        {
            StringBuilder groupByPartBuilder = new StringBuilder();
            foreach (string dimensionName in dimensionFields)
            {
                AnalyticDimension groupDimension = this._dimensions[dimensionName];

                if (groupDimension.Config.JoinConfigNames != null)
                {
                    foreach (var join in groupDimension.Config.JoinConfigNames)
                    {
                        includeJoinConfigNames.Add(join);
                    }
                }
                if (!String.IsNullOrEmpty(groupDimension.Config.IdColumn))
                {
                    AddColumnToStringBuilder(groupByPartBuilder, groupDimension.Config.IdColumn);
                    AddColumnToStringBuilder(selectPartBuilder, String.Format("{0} AS {1}", groupDimension.Config.IdColumn, GetDimensionIdColumnAlias(groupDimension)));
                }

                if (!String.IsNullOrEmpty(groupDimension.Config.NameColumn))
                {
                    AddColumnToStringBuilder(groupByPartBuilder, groupDimension.Config.NameColumn);
                    AddColumnToStringBuilder(selectPartBuilder, String.Format("{0} AS {1}", groupDimension.Config.NameColumn, GetDimensionNameColumnAlias(groupDimension)));
                }
            }
            return groupByPartBuilder.ToString();
        }

        private string BuildQueryJoins( HashSet<string> includeJoinConfigNames)
        {
            StringBuilder joinPartBuilder = new StringBuilder();
            foreach (string joinName in includeJoinConfigNames)
            {
                AnalyticJoin join = _joins[joinName];
                AddStatementToJoinPart(joinPartBuilder, join.Config.JoinStatement);
            }
            return joinPartBuilder.ToString();
        }

        private void BuildQueryMeasures(StringBuilder selectPartBuilder, List<string> allIncludedDimensions, List<string> measureFields, HashSet<string> includeJoinConfigNames)
        {
            Func<AnalyticMeasure, IGetMeasureExpressionContext, string> getMeasureExpression = (measure, getMeasureExpressionContext) =>
            {
                return !string.IsNullOrWhiteSpace(measure.Config.SQLExpression) ? measure.Config.SQLExpression : measure.Evaluator.GetMeasureExpression(getMeasureExpressionContext);
            };


            Func<string, IGetMeasureExpressionContext, string> getMeasureExpressionByMeasureName = (measureName, getMeasureExpressionContext) =>
            {
                AnalyticMeasure measure;
                if (!this._measures.TryGetValue(measureName, out measure))
                    throw new NullReferenceException(String.Format("measure. Name '{0}'", measureName));
                return getMeasureExpression(measure, getMeasureExpressionContext);
            };

            Func<string, bool> isGroupingDimensionIncluded = (dimensionName) => allIncludedDimensions != null && allIncludedDimensions.Contains(dimensionName);

            HashSet<string> addedMeasureColumns = new HashSet<string>();
            foreach (var measureName in measureFields)
            {
                AnalyticMeasure measure = _measures[measureName];

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
                    AnalyticDimension filterDimension = _dimensions[dimensionFilter.Dimension];

                    if (!String.IsNullOrEmpty(filterDimension.Config.IdColumn))
                        AddFilterToFilterPart(filterPartBuilder, dimensionFilter.FilterValues, filterDimension.Config.IdColumn);

                    if (filterDimension.Config.JoinConfigNames != null)
                    {
                        foreach (var join in filterDimension.Config.JoinConfigNames)
                        {
                            includeJoinConfigNames.Add(join);
                        }
                    }
                }
            }
            return filterPartBuilder.ToString();
        }

        private StringBuilder BuildGlobalQuery(DateTime fromTime, DateTime toTime, int? topRecords, Dictionary<string, Object> parameterValues, string selectPartBuilder, string joinPartBuilder, string filterPartBuilder, string groupByPartBuilder, ref int parameterIndex)
        {
            List<TimeRangeTableName> timePeriodTableNames = GetTimeRangeTableNames(fromTime, toTime);

            StringBuilder queryBuilder = new StringBuilder(@"SELECT #TOPRECORDS# #SELECTPART# FROM
                                                                #QUERYBODY#
                                                                #JOINPART#
			                                                    #GROUPBYPART#");

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
            queryBuilder.Replace("#TOPRECORDS#", string.Format("{0}", topRecords.HasValue ? "TOP(" + topRecords.Value + ") " : ""));
            queryBuilder.Replace("#SELECTPART#", selectPartBuilder.ToString());
            if (groupByPartBuilder.Length > 0)
                queryBuilder.Replace("#GROUPBYPART#", "GROUP BY " + groupByPartBuilder);
            else
                queryBuilder.Replace("#GROUPBYPART#", "");
            return queryBuilder;
        }

        private string BuildQueryGrouping(StringBuilder selectPartBuilder, TimeGroupingUnit timeGroupingUnit)
        {
            StringBuilder queryGrouping = new StringBuilder();
            switch(timeGroupingUnit)
            {
                case TimeGroupingUnit.Hour: selectPartBuilder.AppendFormat(" CONVERT(varchar(13), {0}, 121) as [Date]", _table.Settings.TimeColumnName); queryGrouping.AppendFormat(" CONVERT(varchar(13), {0}, 121)", _table.Settings.TimeColumnName); break;
                case TimeGroupingUnit.Day: selectPartBuilder.AppendFormat(" CONVERT(varchar(10), {0}, 121)  as [Date]", _table.Settings.TimeColumnName); queryGrouping.AppendFormat(" CONVERT(varchar(10), {0}, 121)", _table.Settings.TimeColumnName); break;
            }
            return queryGrouping.ToString();
        }

        string GetSingleTableQueryBody(TimeRangeTableName timeRangeTableName, string filterPart, string joinPart, ref int parameterIndex, Dictionary<string, Object> parameterValues)
        {
            StringBuilder singleTableQueryBodyBuilder = new StringBuilder(@" #TABLENAME# ant WITH(NOLOCK)  
                                                                              #JOINPART#                                                                
			                                                                WHERE
			                                                               (#TIMECOLUMNNAME# >= #FromTime#  AND  (#TIMECOLUMNNAME# <= #ToTime# or #ToTime# IS NULL))
                                                                            #FILTERPART#");
            singleTableQueryBodyBuilder.Replace("#TIMECOLUMNNAME#", _table.Settings.TimeColumnName);
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

        string GenerateParameterName(ref int parameterIndex)
        {
            return String.Format("@Prm_{0}", parameterIndex++);
        }

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
                TableName = this._table.Settings.TableName,
                GetPeriodStart = (tr) => tr.FromTime,
                RangeMinLength = TimeSpan.FromSeconds(0),
                IncrementPeriod = (d, tr) => d.Add((tr.ToTime - d))
            };
            var hourlyAllocationContext = new TimeRangeTableAllocationContext
            {
                TableName = this._table.Settings.HourlyTableName,
                GetPeriodStart = GetStartOfHour,
                RangeMinLength = TimeSpan.FromHours(1),
                IncrementPeriod = (d, tr) => d.AddHours(1),
                NextTimeRangeAllocationContext = noTimeAllocationContext
            };
            var dailyAllocationContext = new TimeRangeTableAllocationContext
            {
                TableName = this._table.Settings.DailyTableName,
                GetPeriodStart = GetStartOfDay,
                RangeMinLength = TimeSpan.FromDays(1),
                IncrementPeriod = (d, tr) => d.AddDays(1),
                NextTimeRangeAllocationContext = hourlyAllocationContext
            };
            var weeklyAllocationContext = new TimeRangeTableAllocationContext
            {
                TableName = this._table.Settings.WeeklyTableName,
                GetPeriodStart = GetStartOfWeek,
                RangeMinLength = TimeSpan.FromDays(7),
                IncrementPeriod = (d, tr) => d.AddDays(7),
                NextTimeRangeAllocationContext = dailyAllocationContext
            };
            var monthlyAllocationContext = new TimeRangeTableAllocationContext
            {
                TableName = this._table.Settings.MonthlyTableName,
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
            DateTime startTime = new DateTime(timeRange.FromTime.Year, 1,1);
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
            if(context.NextTimeRangeAllocationContext != null && notAllocatedRanges != null && notAllocatedRanges.Count > 0 )
            {
                foreach(var subRange in notAllocatedRanges)
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

        string GetDimensionIdColumnAlias(AnalyticDimension dimension)
        {
            return String.Format("Dimension_{0}_Id", dimension.AnalyticDimensionConfigId);
        }
        string GetDimensionNameColumnAlias(AnalyticDimension dimension)
        {
            return String.Format("Dimension_{0}_Name", dimension.AnalyticDimensionConfigId);
        }
        string GetMeasureColumnAlias(AnalyticMeasure measure)
        {
            return String.Format("Measure_{0}", measure.AnalyticMeasureConfigId);
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
        void AddStatementToStringBuilder(StringBuilder builder, string statement)
        {
            if (statement.Length != 0)
                builder.Append(" " + statement + " , ");
        }
        void AddStatementToJoinPart(StringBuilder joinPartBuilder, string statement)
        {
            joinPartBuilder.Append(" " + statement + " ");
        }
        void AddPartOfJoinStatment(StringBuilder joinPartBuilder, string statement)
        {
            if (joinPartBuilder != null && joinPartBuilder.Length > 0)
                joinPartBuilder.Append("AND");
            joinPartBuilder.Append(" " + statement + " ");
        }

        #endregion

        #region Mappers
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
                        var dimension = _dimensions[dimensionName];
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

        TimeVariationAnalyticRecord TimeVariationAnalyticRecordMapper(IDataReader reader, TimeVariationAnalyticQuery timeVariationAnalyticQuery, bool isSummary,TimeGroupingUnit timeGroupingUnit)
        {
            TimeVariationAnalyticRecord record = new TimeVariationAnalyticRecord();
            string dateTime = reader["Date"] as string;
            DateTime dateTimeParsedValue = new DateTime();
            switch(timeGroupingUnit)
            {
                case TimeGroupingUnit.Day: DateTime.TryParseExact(dateTime,"yyyy-MM-dd", CultureInfo.CurrentCulture, DateTimeStyles.None, out dateTimeParsedValue); break;
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
                var measure = _measures[measureName];
                measureValues.Add(measureName, GetReaderValue<Object>(reader, GetMeasureColumnAlias(measure)));
            }
            return measureValues;
        }



        #endregion

        #endregion

        #region IAnalyticDataManager Implementation

        AnalyticTable _table;
        public AnalyticTable Table
        {
            set { _table = value; }
        }

        Dictionary<string, AnalyticDimension> _dimensions;
        public Dictionary<string, AnalyticDimension> Dimensions
        {
            set { _dimensions = value; }
        }

        Dictionary<string, AnalyticMeasure> _measures;
        public Dictionary<string, AnalyticMeasure> Measures
        {
            set { _measures = value; }
        }

        Dictionary<string, AnalyticJoin> _joins;
        public Dictionary<string, AnalyticJoin> Joins
        {
            set { _joins = value; }
        }

        #endregion

        #region Overriden Methods
        protected override string GetConnectionString()
        {
            return _table.Settings.ConnectionString;
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
