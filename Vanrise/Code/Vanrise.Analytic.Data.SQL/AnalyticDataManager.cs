using System;
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
        public AnalyticSummaryBigResult<AnalyticRecord> GetFilteredAnalyticRecords(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input)
        {

            Action<string> createTempTableIfNotExistsAction = (tempTableName) =>
            {
                string query = BuildAnalyticQuery(input, tempTableName, false);

                ExecuteNonQueryText(query, (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@FromDate", input.Query.FromTime));
                    cmd.Parameters.Add(new SqlParameter("@ToDate", ToDBNullIfDefault(input.Query.ToTime)));
                    if (input.Query.CurrencyId.HasValue)
                        cmd.Parameters.Add(new SqlParameter("@Currency", input.Query.CurrencyId.Value));
                });
            };

            Dictionary<string, string> columnsMappings = new Dictionary<string, string>();
            for (int i = 0; i < input.Query.DimensionFields.Count; i++)
            {
                var dimensionName = input.Query.DimensionFields[i];
                var dimension = _dimensions[dimensionName];
                columnsMappings.Add(String.Format("DimensionValues[{0}].Name", i), GetDimensionNameColumnAlias(dimension));
            }

            for (int i = 0; i < input.Query.MeasureFields.Count; i++)
            {
                var measureName = input.Query.MeasureFields[i];
                var measure = _measures[measureName];
                columnsMappings.Add(String.Format("MeasureValues.{0}", input.Query.MeasureFields[i]), GetMeasureColumnAlias(measure));
            }

            var rslt = RetrieveData(input, createTempTableIfNotExistsAction, (reader) => AnalyticRecordMapper(reader, input.Query, false)
            , columnsMappings, new AnalyticSummaryBigResult<AnalyticRecord>()) as AnalyticSummaryBigResult<AnalyticRecord>;
            if (input.Query.WithSummary)
                rslt.Summary = GetSummary(input);
            return rslt;
        }

        #region Private Methods
        AnalyticRecord GetSummary(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input)
        {
            string query = BuildAnalyticQuery(input, "AnalyticSummaryTable", input.Query.WithSummary);
            return GetItemText(query, reader => AnalyticRecordMapper(reader, input.Query, true), (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@FromDate", input.Query.FromTime));
                    cmd.Parameters.Add(new SqlParameter("@ToDate", ToDBNullIfDefault(input.Query.ToTime)));
                    if (input.Query.CurrencyId.HasValue)
                        cmd.Parameters.Add(new SqlParameter("@Currency", input.Query.CurrencyId.Value));
                });
        }

        #region Query Builder
        string BuildAnalyticQuery(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input, string tempTableName, bool isSummary)
        {
            StringBuilder selectPartBuilder = new StringBuilder();
            StringBuilder joinPartBuilder = new StringBuilder();
            StringBuilder filterPartBuilder = new StringBuilder();
            StringBuilder groupByPartBuilder = new StringBuilder();
            StringBuilder ctePartBuilder = new StringBuilder();

            // List<string> lstCTEStatements = new List<string>();
            HashSet<string> includeJoinConfigNames = new HashSet<string>();
            //List<string> lstWhereStatement = new List<string>();

            StringBuilder selectBodyBuilder = new StringBuilder(@"SELECT #TOPRECORDS# #SELECTPART#
			                                                    FROM #TABLENAME# ant --WITH(NOLOCK ,INDEX(#TABLEINDEX#))
                                                                #JOINPART#
                                                                #EXCHANGEJOINPART#
			                                                    WHERE
			                                                   (FirstCDRAttempt >= @FromDate  AND  (FirstCDRAttempt <=@ToDate or @ToDate IS NULL))
                                                                #FILTERPART#
			                                                    #GROUPBYPART#");

            #region MainQuery
            StringBuilder queryBuilder = new StringBuilder(@"IF NOT OBJECT_ID('#TEMPTABLE#', N'U') IS NOT NULL
	                                                        BEGIN
                                                                ;WITH 
                                                                --#CTEPART#
                                                                AllResult AS( 
                                                                    #SELECTBODYPART#
                                                                    )
                                                                SELECT * INTO #TEMPTABLE# FROM AllResult
                                                            END ");
            #endregion

            #region Adding Group Fields Part
            if (!isSummary)
                foreach (string dimensionName in input.Query.DimensionFields)
                {
                    AnalyticDimension groupDimension = this._dimensions[dimensionName];

                    if (groupDimension.Config.GroupByColumns != null)
                    {
                        foreach (var column in groupDimension.Config.GroupByColumns)
                        {
                            AddColumnToStringBuilder(groupByPartBuilder, column);
                        }
                    }
                    if (groupDimension.Config.JoinConfigNames != null)
                    {
                        foreach (var join in groupDimension.Config.JoinConfigNames)
                        {
                            includeJoinConfigNames.Add(join);
                        }
                    }
                    if (!String.IsNullOrEmpty(groupDimension.Config.IdColumn))
                        AddColumnToStringBuilder(selectPartBuilder, String.Format("{0} AS {1}", groupDimension.Config.IdColumn, GetDimensionIdColumnAlias(groupDimension)));

                    if (!String.IsNullOrEmpty(groupDimension.Config.NameColumn))
                        AddColumnToStringBuilder(selectPartBuilder, String.Format("{0} AS {1}", groupDimension.Config.NameColumn, GetDimensionNameColumnAlias(groupDimension)));

                }
            #endregion

            #region Adding FilterPart

            if (input.Query.Filters != null)
            {
                foreach (DimensionFilter dimensionFilter in input.Query.Filters)
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

            #endregion

            #region Add Measures Part
            HashSet<string> addedMeasureColumns = new HashSet<string>();
            foreach (var measureName in input.Query.MeasureFields)
            {
                AnalyticMeasure measure = _measures[measureName];

                GetMeasureExpressionContext getMeasureExpressionContext = new GetMeasureExpressionContext();
                string measureExpression = !string.IsNullOrWhiteSpace(measure.Config.SQLExpression) ? measure.Config.SQLExpression : measure.Evaluator.GetMeasureExpression(getMeasureExpressionContext);
                AddColumnToStringBuilder(selectPartBuilder, String.Format("{0} AS {1}", measureExpression, GetMeasureColumnAlias(measure)));

                if (measure.Config.JoinConfigNames != null)
                {
                    foreach (var join in measure.Config.JoinConfigNames)
                    {
                        includeJoinConfigNames.Add(join);
                    }
                }
            }

            #endregion

            #region Adding JoinPart


            foreach (string joinName in includeJoinConfigNames)
            {
                AnalyticJoin join = _joins[joinName];
                AddStatementToJoinPart(joinPartBuilder, join.Config.JoinStatement);
            }


            #endregion

            #region Query Replacement

            selectBodyBuilder.Replace("#TOPRECORDS#", string.Format("{0}", input.Query.TopRecords != null ? "TOP(" + input.Query.TopRecords + ") " : ""));
            selectBodyBuilder.Replace("#TABLENAME#", _table.Settings.TableName);
            selectBodyBuilder.Replace("#TABLEINDEX#", "");
            selectBodyBuilder.Replace("#SELECTPART#", selectPartBuilder.ToString());
            selectBodyBuilder.Replace("#JOINPART#", joinPartBuilder.ToString());
            selectBodyBuilder.Replace("#FILTERPART#", filterPartBuilder.ToString());
            selectBodyBuilder.Replace("#EXCHANGEJOINPART#", "");
            queryBuilder.Replace("#TEMPTABLE#", tempTableName);
            if (groupByPartBuilder.Length > 0)
                selectBodyBuilder.Replace("#GROUPBYPART#", "GROUP BY " + groupByPartBuilder);
            else
                selectBodyBuilder.Replace("#GROUPBYPART#", "");

            #endregion

            return isSummary ? selectBodyBuilder.ToString() : queryBuilder.Replace("#SELECTBODYPART#", selectBodyBuilder.ToString()).ToString();
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
                        if (dimensionId != null)
                        {
                            record.DimensionValues[index] = new DimensionValue
                            {
                                Value = dimensionId,
                                Name = reader[GetDimensionNameColumnAlias(dimension)] as string
                            };
                        }

                        index++;
                    }
                }
            }
            index = 0;
            foreach (var measureName in analyticQuery.MeasureFields)
            {
                var measure = _measures[measureName];
                record.MeasureValues.Add(measureName, GetReaderValue<Object>(reader, GetMeasureColumnAlias(measure)));
                index++;
            }
            index = 0;
            return record;
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
    }
}
