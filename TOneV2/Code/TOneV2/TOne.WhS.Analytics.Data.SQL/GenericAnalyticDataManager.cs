using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.Analytics.Entities;

namespace TOne.WhS.Analytics.Data.SQL
{
    public class GenericAnalyticDataManager : BaseTOneDataManager, IGenericAnalyticDataManager
    {

        #region ctor/Local Variables
        public GenericAnalyticDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        { }
        static string trafficStatsTableName = "TOneWhS_Analytic.TrafficStats";
        static string trafficStatsByCodeTableName = "TOneWhS_Analytic.TrafficStatsByCode";
        #endregion

        #region Public Methods
        public AnalyticSummaryBigResult<AnalyticRecord> GetAnalyticRecords(Vanrise.Entities.DataRetrievalInput<GenericAnalyticQuery> input)
        {

            string tempTable = null;
            List<AnalyticDimension> fields = new List<AnalyticDimension>();
            if (input.Query.Filters != null)
            {
                foreach (DimensionFilter add in input.Query.Filters)
                {
                    fields.Add(add.Dimension);
                }
            }

            GenericAnalyticConfigManager _manager = new GenericAnalyticConfigManager();
            Dictionary<AnalyticDimension, AnalyticDimensionConfig> dimensionsConfig = _manager.GetGroupFieldsConfig(input.Query.DimensionFields);
            Dictionary<AnalyticDimension, AnalyticDimensionConfig> dimensionsFilterConfig = _manager.GetGroupFieldsConfig(fields);
            Dictionary<AnalyticMeasureField, AnalyticMeasureFieldConfig> measureFieldsConfig = _manager.GetMeasureFieldsConfig(input.Query.MeasureFields);

            Action<string> createTempTableIfNotExistsAction = (tempTableName) =>
            {
                tempTable = tempTableName;
                string query = BuildAnalyticSummaryQuery(input, tempTableName, dimensionsConfig, measureFieldsConfig, dimensionsFilterConfig);

                if (input.Query.Currency != null)
                    ExecuteNonQueryText(query, (cmd) =>
                    {
                        cmd.Parameters.Add(new SqlParameter("@FromDate", input.Query.FromTime));
                        cmd.Parameters.Add(new SqlParameter("@ToDate", input.Query.ToTime));
                        cmd.Parameters.Add(new SqlParameter("@Currency", input.Query.Currency));
                    });
                else
                    ExecuteNonQueryText(query, (cmd) =>
                    {
                        cmd.Parameters.Add(new SqlParameter("@FromDate", input.Query.FromTime));
                        cmd.Parameters.Add(new SqlParameter("@ToDate", input.Query.ToTime));
                    });
            };

            Dictionary<string, string> columnsMappings = new Dictionary<string, string>();
            for (int i = 0; i < input.Query.DimensionFields.Length; i++)
            {
                var groupField = input.Query.DimensionFields[i];
                columnsMappings.Add(String.Format("DimensionValues[{0}].Name", i), String.Format("DimensionName_{0}", groupField));
            }

            for (int i = 0; i < input.Query.MeasureFields.Length; i++)
            {
                var measureField = input.Query.MeasureFields[i];
                var measureFieldConfig = measureFieldsConfig[measureField];
                if (measureFieldConfig.MappedSQLColumn != null)
                    columnsMappings.Add(String.Format("MeasureValues.{0}", input.Query.MeasureFields[i]), measureFieldConfig.MappedSQLColumn);
            }

            AnalyticSummaryBigResult<AnalyticRecord> rslt = RetrieveData(input, createTempTableIfNotExistsAction, (reader) => AnalyticRecordMapper(reader, dimensionsConfig, measureFieldsConfig)
            , columnsMappings, new AnalyticSummaryBigResult<AnalyticRecord>()) as AnalyticSummaryBigResult<AnalyticRecord>;

            if (input.Query.WithSummary)
                rslt.Summary = GetSummary(input, tempTable, dimensionsConfig, measureFieldsConfig);
            return rslt;
        }

        #endregion

        #region Private Methods
        private AnalyticRecord GetSummary(Vanrise.Entities.DataRetrievalInput<GenericAnalyticQuery> input, string tempTableName,
          Dictionary<AnalyticDimension, AnalyticDimensionConfig> dimensionsConfig,
          Dictionary<AnalyticMeasureField, AnalyticMeasureFieldConfig> measureFieldsConfig)
        {

            StringBuilder queryBuilder = new StringBuilder(@"SELECT #SELECTPART# FROM #TEMPTABLE# ts");
            StringBuilder selectPartBuilder = new StringBuilder();

            //adding Measures related parts to the query
            #region Adding Measure Related Parts
            List<string> addedMeasureColumns = new List<string>();
            foreach (AnalyticMeasureField measureField in input.Query.MeasureFields)
            {
                AnalyticMeasureFieldConfig measureFieldConfig = measureFieldsConfig[measureField];
                if (measureFieldConfig.GetColumnsExpressions != null)
                {
                    foreach (var exp in measureFieldConfig.GetColumnsExpressions)
                    {
                        var measureColumn = exp(input.Query);
                        if (!addedMeasureColumns.Contains(measureColumn.ColumnAlias))
                        {
                            AddColumnToStringBuilder(selectPartBuilder,
                                String.Format("{0}({1}) AS {1}", measureColumn.ExpressionSummary, measureColumn.ColumnAlias));

                            addedMeasureColumns.Add(measureColumn.ColumnAlias);
                        }
                    }
                }
            }
            #endregion

            queryBuilder.Replace("#TEMPTABLE#", tempTableName);
            queryBuilder.Replace("#SELECTPART#", selectPartBuilder.ToString());

            return GetItemText(queryBuilder.ToString(), (reader) => AnalyticRecordMapper(reader, null, measureFieldsConfig), null);
        }


        private string BuildAnalyticSummaryQuery(Vanrise.Entities.DataRetrievalInput<GenericAnalyticQuery> input, string tempTableName,
            Dictionary<AnalyticDimension, AnalyticDimensionConfig> dimensionsConfig,
            Dictionary<AnalyticMeasureField, AnalyticMeasureFieldConfig> measureFieldsConfig,
            Dictionary<AnalyticDimension, AnalyticDimensionConfig> dimensionsFilterConfig)
        {

            StringBuilder selectPartBuilder = new StringBuilder();
            StringBuilder joinPartBuilder = new StringBuilder();
            StringBuilder filterPartBuilder = new StringBuilder();
            StringBuilder groupByPartBuilder = new StringBuilder();
            StringBuilder ctePartBuilder = new StringBuilder();
             StringBuilder billingJoinPartBuilder = new StringBuilder();

            string tableNamePartBuilder = trafficStatsTableName;
            string tableIndex = "IX_TrafficStats_DateTimeFirst";
            List<string> lstCTEStatements = new List<string>();
            List<string> lstJoinStatement = new List<string>();
            List<string> lstWhereStatement = new List<string>();

            #region MainQuery
            StringBuilder queryBuilder = new StringBuilder(@"IF NOT OBJECT_ID('#TEMPTABLE#', N'U') IS NOT NULL
	                                                        BEGIN
                                                                ;WITH 
                                                                #CTEPART#
                                                                AllResult AS( 
			                                                    SELECT #TOPRECORDS# #SELECTPART#
			                                                    FROM #TABLENAME# ts WITH(NOLOCK ,INDEX(#TABLEINDEX#))
                                                                #JOINPART#
			                                                    WHERE
			                                                    FirstCDRAttempt BETWEEN @FromDate AND @ToDate
                                                                #FILTERPART#
			                                                    #GROUPBYPART#)
                                                                SELECT * INTO #TEMPTABLE# FROM AllResult
                                                            END ");
            #endregion
            //adding group fields related parts to the query
            #region Adding Group Fields Part
            foreach (AnalyticDimension groupField in input.Query.DimensionFields)
            {
                AnalyticDimensionConfig groupFieldConfig = dimensionsConfig[groupField];
                switch (groupField)
                {
                    case AnalyticDimension.Zone:AddPartOfJoinStatment(billingJoinPartBuilder," ts.SaleZoneID=BS.SaleZoneID "); break;
                    case AnalyticDimension.SupplierZone: AddPartOfJoinStatment(billingJoinPartBuilder," ts.SupplierZoneID=BS.SupplierZoneID "); break;
                    case AnalyticDimension.Supplier:AddPartOfJoinStatment(billingJoinPartBuilder," ts.SupplierID=BS.SupplierID "); break;
                    case AnalyticDimension.Customer: AddPartOfJoinStatment(billingJoinPartBuilder," ts.CustomerID=BS.CustomerID "); break;
                }
               

                if (groupFieldConfig.GroupByStatements != null)
                    foreach (var statement in groupFieldConfig.GroupByStatements)
                    {
                        AddColumnToStringBuilder(groupByPartBuilder, statement);
                    }

                if (groupFieldConfig.JoinStatements != null)
                    foreach (var statement in groupFieldConfig.JoinStatements)
                    {
                        if (!lstJoinStatement.Contains(statement))
                        {
                            lstJoinStatement.Add(statement);
                            AddStatementToJoinPart(joinPartBuilder, statement);
                        }
                    }

                //if (groupField == AnalyticDimension.Country)
                //{
                //    if (!lstCTEStatements.Contains(CTEStatement.Country))
                //    {
                //        lstCTEStatements.Add(CTEStatement.Country);
                //        AddStatementToStringBuilder(ctePartBuilder, CTEStatement.Country);
                //    }
                //}

                //if (groupField == AnalyticDimension.GateWayIn || groupField == AnalyticDimension.GateWayOut)
                //{
                //    if (!lstCTEStatements.Contains(CTEStatement.SwitchConnectivity))
                //    {
                //        lstCTEStatements.Add(CTEStatement.SwitchConnectivity);
                //        AddStatementToCTEPart(ctePartBuilder, CTEStatement.SwitchConnectivity);
                //    }
                //}

                if (!String.IsNullOrEmpty(groupFieldConfig.IdColumn))
                    AddColumnToStringBuilder(selectPartBuilder, String.Format("{0} AS DimensionId_{1}", groupFieldConfig.IdColumn, groupField));

                if (!String.IsNullOrEmpty(groupFieldConfig.NameColumn))
                    AddColumnToStringBuilder(selectPartBuilder, String.Format("{0} AS DimensionName_{1}", groupFieldConfig.NameColumn, groupField));

                if (groupField == AnalyticDimension.CodeSales || groupField == AnalyticDimension.CodeBuy)
                    tableNamePartBuilder = trafficStatsByCodeTableName;
            }
          
            #endregion

            #region Adding FilterPart
            if (input.Query.Filters != null)
            {
                foreach (DimensionFilter dimensionFilter in input.Query.Filters)
                {

                    if (dimensionFilter.Dimension == AnalyticDimension.CodeSales || dimensionFilter.Dimension == AnalyticDimension.CodeBuy)
                        tableNamePartBuilder = trafficStatsByCodeTableName;

                    AnalyticDimensionConfig filterFieldConfig = dimensionsFilterConfig[dimensionFilter.Dimension];

                    if (!String.IsNullOrEmpty(filterFieldConfig.IdColumn))
                        AddFilterToFilterPart(filterPartBuilder, dimensionFilter.FilterValues, filterFieldConfig.IdColumn);

                    if (dimensionsFilterConfig.ContainsKey(dimensionFilter.Dimension))
                    {
                        AnalyticDimensionConfig groupFieldConfig = dimensionsFilterConfig[dimensionFilter.Dimension];

                        //if (dimensionFilter.Dimension == AnalyticDimension.Country)
                        //{
                        //    if (!lstCTEStatements.Contains(CTEStatement.Country))
                        //    {
                        //        lstCTEStatements.Add(CTEStatement.Country);
                        //        AddStatementToStringBuilder(ctePartBuilder, CTEStatement.Country);
                        //    }
                        //}
                        if (groupFieldConfig.JoinStatements != null)
                            foreach (var statement in groupFieldConfig.JoinStatements)
                            {
                                if (!lstJoinStatement.Contains(statement))
                                {
                                    lstJoinStatement.Add(statement);
                                    AddStatementToJoinPart(joinPartBuilder, statement);
                                }
                            }
                    }
                }
            }
            #endregion
            //adding Measures related parts to the query
            #region Adding Measures Related Part 
            List<string> addedMeasureColumns = new List<string>();
            foreach (AnalyticMeasureField measureField in input.Query.MeasureFields)
            {
                AnalyticMeasureFieldConfig measureFieldConfig = measureFieldsConfig[measureField];
                if (measureFieldConfig.GetColumnsExpressions != null)
                {
                    foreach (var exp in measureFieldConfig.GetColumnsExpressions)
                    {
                        var measureColumn = exp(input.Query);
                        if (!addedMeasureColumns.Contains(measureColumn.ColumnAlias))
                        {
                            AddColumnToStringBuilder(selectPartBuilder, String.Format("{0} AS {1}", measureColumn.Expression, measureColumn.ColumnAlias));
                            if (measureColumn.JoinStatement != null)
                            {
                                if (!lstJoinStatement.Contains(measureColumn.JoinStatement))
                                {
                                    lstJoinStatement.Add(measureColumn.JoinStatement);
                                    AddStatementToJoinPart(joinPartBuilder, measureColumn.JoinStatement);
                                }
                            }

                            //Measures related to Billing
                            if (measureField == AnalyticMeasureField.BillingNumberOfCalls || measureField == AnalyticMeasureField.CostNets ||
                                measureField == AnalyticMeasureField.SaleNets || measureField == AnalyticMeasureField.Profit || measureField == AnalyticMeasureField.PricedDuration || measureField == AnalyticMeasureField.CostRate || measureField == AnalyticMeasureField.SaleRate)
                            {
                                if (!lstCTEStatements.Contains(CTEStatement.Billing))
                                {

                                    if (input.Query.Currency != null)
                                    {

                                        lstCTEStatements.Add(CTEStatement.ConvertedToCurrency);
                                        AddStatementToStringBuilder(ctePartBuilder, CTEStatement.ConvertedToCurrency);
                                    }
                                    else
                                    {
                                        lstCTEStatements.Add(CTEStatement.ExchangeCurrency);
                                        AddStatementToStringBuilder(ctePartBuilder, CTEStatement.ExchangeCurrency);
                                    }
                               //     lstCTEStatements.Add(CTEStatement.Country);
                                    lstCTEStatements.Add(CTEStatement.Billing);
                                //    AddStatementToStringBuilder(ctePartBuilder, CTEStatement.Country);
                                    AddStatementToStringBuilder(ctePartBuilder, CTEStatement.Billing);


                                }
                                if (!lstJoinStatement.Contains(JoinStatement.Billing))
                                {
                                    lstJoinStatement.Add(JoinStatement.Billing);
                                    AddStatementToJoinPart(joinPartBuilder, JoinStatement.Billing);
                                }
                                if (!lstWhereStatement.Contains(WhereStatement.Billing))
                                {
                                    lstWhereStatement.Add(WhereStatement.Billing);
                                    AddStatementToStringBuilder(filterPartBuilder, WhereStatement.Billing);
                                }
                            }

                            addedMeasureColumns.Add(measureColumn.ColumnAlias);
                        }
                    }
                }
            }
            #endregion

            #region Replacment Query
            
            queryBuilder.Replace("#TOPRECORDS#",string.Format("{0}",input.Query.TopRecords!=null?"TOP("+input.Query.TopRecords+") ":""));
            queryBuilder.Replace("#TABLENAME#", tableNamePartBuilder);
            queryBuilder.Replace("#TEMPTABLE#", tempTableName);
            queryBuilder.Replace("#TABLEINDEX#", tableIndex);
            queryBuilder.Replace("#SELECTPART#", selectPartBuilder.ToString());
            queryBuilder.Replace("#JOINPART#", joinPartBuilder.ToString());
            queryBuilder.Replace("#FILTERPART#", filterPartBuilder.ToString());

            if (groupByPartBuilder.Length > 0)
                queryBuilder.Replace("#GROUPBYPART#", "GROUP BY " + groupByPartBuilder);
            else
                queryBuilder.Replace("#GROUPBYPART#", "");
            queryBuilder.Replace("#CTEPART#", ctePartBuilder.ToString());
            if (input.Query.Currency != null)
                queryBuilder.Replace("#EXCHANGEJOINPART#", JoinStatement.ConvertedToCurrency);
            else
                queryBuilder.Replace("#EXCHANGEJOINPART#", JoinStatement.ExchangeCurrency);
            
            queryBuilder.Replace("#BILLINGJOINSTATEMENT#", billingJoinPartBuilder.ToString());
            
            #endregion

            return queryBuilder.ToString();
        }

        private void AddFilterToFilterPart<T>(StringBuilder filterBuilder, List<T> values, string column)
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

        private AnalyticRecord AnalyticRecordMapper(IDataReader reader, Dictionary<AnalyticDimension, AnalyticDimensionConfig> dimensionsConfig, Dictionary<AnalyticMeasureField, AnalyticMeasureFieldConfig> measureFieldsConfig)
        {
            AnalyticRecord record = new AnalyticRecord()
            {
                DimensionValues = dimensionsConfig != null ? new AnalyticDimensionValue[dimensionsConfig.Count] : new AnalyticDimensionValue[0],
                MeasureValues = new Dictionary<AnalyticMeasureField, object>() // Object[measureFieldsConfig.Count]
            };
            var index = 0;
            if (dimensionsConfig != null)
                foreach (var groupFieldConfig in dimensionsConfig)
                {
                    object id = GetReaderValue<object>(reader, String.Format("DimensionId_{0}", groupFieldConfig.Key));
                    if (id != null)
                    {
                        string nameColumnName = String.Format("DimensionName_{0}", groupFieldConfig.Key);
                        record.DimensionValues[index] = new AnalyticDimensionValue
                        {
                            Id = id,
                            Name = reader[nameColumnName].ToString()
                        };
                    }

                    index++;
                }
            index = 0;
            foreach (var measureFieldConfig in measureFieldsConfig)
            {
                if (measureFieldConfig.Value.GetMeasureValue != null)
                    record.MeasureValues[measureFieldConfig.Key] = measureFieldConfig.Value.GetMeasureValue(reader, record);
                index++;
            }
            index = 0;
            return record;
        }
        
        #endregion

    }
}
