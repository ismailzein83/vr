using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;
using TOne.Data.SQL;

namespace TOne.Analytics.Data.SQL
{
    public class GenericAnalyticDataManager : BaseTOneDataManager, IGenericAnalyticDataManager
    {
        public Vanrise.Entities.BigResult<AnalyticRecord> GetAnalyticRecords(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input)
        {
            GenericAnalyticConfigManager _manager = new GenericAnalyticConfigManager();
            Dictionary<AnalyticDimension, AnalyticDimensionConfig> dimensionsConfig = _manager.GetGroupFieldsConfig(input.Query.DimensionFields);

            Dictionary<AnalyticDimension, AnalyticDimensionConfig> dimensionsFilterConfig = new Dictionary<AnalyticDimension,AnalyticDimensionConfig>();

            List<AnalyticDimension> fields = new List<AnalyticDimension>();
            foreach (DimensionFilter add in input.Query.Filters)
            {
                fields.Add(add.Dimension);
            }
            dimensionsFilterConfig = _manager.GetGroupFieldsConfig(fields);

            Dictionary<AnalyticMeasureField, AnalyticMeasureFieldConfig> measureFieldsConfig = _manager.GetMeasureFieldsConfig(input.Query.MeasureFields);

            Action<string> createTempTableIfNotExistsAction = (tempTableName) =>
            {
                string query = BuildAnalyticSummaryQuery(input, tempTableName, dimensionsConfig, measureFieldsConfig, dimensionsFilterConfig);
                ExecuteNonQueryText(query, (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@FromDate", input.Query.FromTime));
                    cmd.Parameters.Add(new SqlParameter("@ToDate", input.Query.ToTime));
                });
            };

            Dictionary<string, string> columnsMappings = new Dictionary<string, string>();
            for (int i = 0; i < input.Query.DimensionFields.Length; i++ )
            {
                var groupField = input.Query.DimensionFields[i];
                columnsMappings.Add(String.Format("DimensionValues[{0}].Name", i), String.Format("DimensionName_{0}", groupField));
            }

            for (int i = 0; i < input.Query.MeasureFields.Length; i++)
            {
                var measureField = input.Query.MeasureFields[i];
                var measureFieldConfig = measureFieldsConfig[measureField];
                if (measureFieldConfig.MappedSQLColumn != null)
                    columnsMappings.Add(String.Format("MeasureValues[{0}]", i), measureFieldConfig.MappedSQLColumn);
            }

            return RetrieveData(input, createTempTableIfNotExistsAction, (reader) => AnalyticRecordMapper(reader, dimensionsConfig, measureFieldsConfig), columnsMappings);
        }

        private string BuildAnalyticSummaryQuery(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input, string tempTableName, 
            Dictionary<AnalyticDimension, AnalyticDimensionConfig> dimensionsConfig, 
            Dictionary<AnalyticMeasureField, AnalyticMeasureFieldConfig> measureFieldsConfig,
            Dictionary<AnalyticDimension, AnalyticDimensionConfig> dimensionsFilterConfig)
        {
            StringBuilder queryBuilder = new StringBuilder(@"IF NOT OBJECT_ID('#TEMPTABLE#', N'U') IS NOT NULL
	                                                        BEGIN
                                                                WITH 
                                                                #CTEPART#
                                                                AllResult AS( 
			                                                    SELECT #SELECTPART#
			                                                    FROM #TABLENAME# ts WITH(NOLOCK ,INDEX(IX_#TABLENAME#_DateTimeFirst))
                                                                #JOINPART#
			                                                    WHERE
			                                                    FirstCDRAttempt BETWEEN @FromDate AND @ToDate
                                                                #FILTERPART#
			                                                    GROUP BY #GROUPBYPART#)
                                                                SELECT * INTO #TEMPTABLE# FROM AllResult
                                                            END ");

            StringBuilder selectPartBuilder = new StringBuilder();
            StringBuilder joinPartBuilder = new StringBuilder();
            StringBuilder filterPartBuilder = new StringBuilder();
            StringBuilder groupByPartBuilder = new StringBuilder();
            StringBuilder ctePartBuilder = new StringBuilder();
            string tableNamePartBuilder = "TrafficStats";

            List<string> lstCTEStatements = new List<string>();
            List<string> lstJoinStatement = new List<string>();
            List<string> lstWhereStatement = new List<string>();
            //adding group fields related parts to the query
            foreach (AnalyticDimension groupField in input.Query.DimensionFields)
            {                
                AnalyticDimensionConfig groupFieldConfig = dimensionsConfig[groupField];

                if (groupFieldConfig.GroupByStatements != null)
                    foreach (var statement in groupFieldConfig.GroupByStatements)
                    {
                        AddColumnToGroupByPart(groupByPartBuilder, statement);
                    }

                if(groupFieldConfig.JoinStatements != null)
                    foreach (var statement in groupFieldConfig.JoinStatements)
                    {
                        AddStatementToJoinPart(joinPartBuilder, statement);
                    }

                if (groupFieldConfig.CTEStatement != null)
                    AddStatementToCTEPart(ctePartBuilder, groupFieldConfig.CTEStatement);
                
                if(!String.IsNullOrEmpty(groupFieldConfig.IdColumn))
                    AddColumnToSelectPart(selectPartBuilder, String.Format("{0} AS DimensionId_{1}", groupFieldConfig.IdColumn, groupField));
                
                if (!String.IsNullOrEmpty(groupFieldConfig.NameColumn))
                    AddColumnToSelectPart(selectPartBuilder, String.Format("{0} AS DimensionName_{1}", groupFieldConfig.NameColumn, groupField));
                if (groupField == AnalyticDimension.CodeSales || groupField == AnalyticDimension.CodeBuy)
                    tableNamePartBuilder = "TrafficStatsByCode";
            }

            //adding Measures related parts to the query
            List<string> addedMeasureColumns = new List<string>();
            foreach (AnalyticMeasureField measureField in input.Query.MeasureFields)
            {
                AnalyticMeasureFieldConfig measureFieldConfig = measureFieldsConfig[measureField];
                if(measureFieldConfig.GetColumnsExpressions != null)
                {
                    foreach(var exp in measureFieldConfig.GetColumnsExpressions)
                    {
                        var measureColumn = exp(input.Query);
                        if(!addedMeasureColumns.Contains(measureColumn.ColumnAlias))
                        {
                            AddColumnToSelectPart(selectPartBuilder, String.Format("{0} AS {1}", measureColumn.Expression, measureColumn.ColumnAlias));
                            if (measureColumn.JoinStatement != null)
                                AddStatementToJoinPart(joinPartBuilder, measureColumn.JoinStatement);


                            //Measures related to Billing
                            if (measureField == AnalyticMeasureField.BillingNumberOfCalls || measureField == AnalyticMeasureField.CostNets ||
                                measureField == AnalyticMeasureField.SaleNets || measureField == AnalyticMeasureField.Profit)
                            {
                                if(!lstCTEStatements.Contains(CTEStatement.Billing))
                                {
                                    lstCTEStatements.Add(CTEStatement.Billing);
                                    AddStatementToCTEPart(ctePartBuilder, CTEStatement.Billing);
                                }
                                if (!lstJoinStatement.Contains(JoinStatement.Billing))
                                {
                                    lstJoinStatement.Add(JoinStatement.Billing);
                                    AddStatementToJoinPart(joinPartBuilder, JoinStatement.Billing);
                                }
                                if (!lstWhereStatement.Contains(WhereStatement.Billing))
                                {
                                    lstWhereStatement.Add(WhereStatement.Billing);
                                    AddFilterToFilterPart(filterPartBuilder, WhereStatement.Billing);
                                }
                            }

                            addedMeasureColumns.Add(measureColumn.ColumnAlias);
                        }
                    }
                }
            }

            foreach(DimensionFilter dimensionFilter in input.Query.Filters)
            {

                if (dimensionFilter.Dimension == AnalyticDimension.CodeSales || dimensionFilter.Dimension == AnalyticDimension.CodeBuy)
                    tableNamePartBuilder = "TrafficStatsByCode";

                AnalyticDimensionConfig filterFieldConfig = dimensionsFilterConfig[dimensionFilter.Dimension];


                if (!String.IsNullOrEmpty(filterFieldConfig.IdColumn))
                    AddFilterToFilterPart(filterPartBuilder, dimensionFilter.FilterValues, filterFieldConfig.IdColumn);

                if (dimensionsFilterConfig.ContainsKey(dimensionFilter.Dimension))
                {
                    AnalyticDimensionConfig groupFieldConfig = dimensionsFilterConfig[dimensionFilter.Dimension];

                    if (groupFieldConfig.CTEStatement != null)
                    {    
                        AddStatementToCTEPart(ctePartBuilder, groupFieldConfig.CTEStatement);

                        if (groupFieldConfig.JoinStatements != null)
                            foreach (var statement in groupFieldConfig.JoinStatements)
                            {
                                AddStatementToJoinPart(joinPartBuilder, statement);
                            }
                    }
                }
            }
            queryBuilder.Replace("#TABLENAME#", tableNamePartBuilder);
            queryBuilder.Replace("#TEMPTABLE#", tempTableName);
            queryBuilder.Replace("#SELECTPART#", selectPartBuilder.ToString());
            queryBuilder.Replace("#JOINPART#", joinPartBuilder.ToString());
            queryBuilder.Replace("#FILTERPART#", filterPartBuilder.ToString());
            queryBuilder.Replace("#GROUPBYPART#", groupByPartBuilder.ToString());
            queryBuilder.Replace("#CTEPART#", ctePartBuilder.ToString());

            return queryBuilder.ToString();
        }

        public void AddFilterToFilterPart<T>(StringBuilder filterBuilder, List<T> values, string column)
        {
            if (values != null && values.Count() > 0)
            {
                if (values[0].GetType() == typeof(string) || values[0].GetType() == typeof(DateTime))
                    filterBuilder.AppendFormat(" AND {0} = '{1}' ", column, String.Join(", ", values));
                else
                    filterBuilder.AppendFormat(" AND {0} = {1} ", column, String.Join(", ", values));
            }
        }
        public void AddFilterToFilterPart(StringBuilder filterBuilder, string statement)
        {
            if (statement.Length != 0)
                filterBuilder.Append(" " + statement + " ");
        }

        void AddColumnToSelectPart(StringBuilder selectPartBuilder, string column)
        {
            if(selectPartBuilder.Length > 0)
            {
                selectPartBuilder.Append(" , ");
            }
            selectPartBuilder.Append(column);
        }

        void AddColumnToGroupByPart(StringBuilder groupByPartBuilder, string column)
        {
            if (groupByPartBuilder.Length > 0)
            {
                groupByPartBuilder.Append(" , ");
            }
            groupByPartBuilder.Append(column);
        }

        void AddStatementToCTEPart(StringBuilder ctePartBuilder, string statement)
        {
            if (statement.Length != 0)
                ctePartBuilder.Append( " " + statement + " , ");
        }

        void AddStatementToJoinPart(StringBuilder joinPartBuilder, string statement)
        {
            joinPartBuilder.Append(" " + statement + " ");
        }

        AnalyticRecord AnalyticRecordMapper(IDataReader reader, Dictionary<AnalyticDimension, AnalyticDimensionConfig> dimensionsConfig, Dictionary<AnalyticMeasureField, AnalyticMeasureFieldConfig> measureFieldsConfig)
        {
            AnalyticRecord record = new AnalyticRecord()
            {
                DimensionValues = new AnalyticDimensionValue[dimensionsConfig.Count],
                MeasureValues = new Object[measureFieldsConfig.Count]
            };

            var index = 0;
            foreach (var groupFieldConfig in dimensionsConfig)
            {
                object id = GetReaderValue<object>(reader, String.Format("DimensionId_{0}", groupFieldConfig.Key));
                if (id != null)
                {
                    string nameColumnName = String.Format("DimensionName_{0}", groupFieldConfig.Key);
                    record.DimensionValues[index] = new AnalyticDimensionValue
                    {
                        Id = id,
                        Name =  reader[nameColumnName].ToString()
                    };
                }

                index++;
            }

            index = 0;

            foreach (var measureFieldConfig in measureFieldsConfig)
            {                
                if (measureFieldConfig.Value.GetMeasureValue != null)
                    record.MeasureValues[index] = measureFieldConfig.Value.GetMeasureValue(reader, record);

                index++;
            }

            return record;
        }
    }
}
