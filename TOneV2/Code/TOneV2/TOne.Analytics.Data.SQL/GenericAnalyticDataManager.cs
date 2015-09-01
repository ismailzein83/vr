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
            Dictionary<AnalyticGroupField, AnalyticGroupFieldConfig> groupFieldsConfig = _manager.GetGroupFieldsConfig(input.Query.GroupFields);
            Dictionary<AnalyticMeasureField, AnalyticMeasureFieldConfig> measureFieldsConfig = _manager.GetMeasureFieldsConfig(input.Query.MeasureFields);

            Action<string> createTempTableIfNotExistsAction = (tempTableName) =>
            {                
                string query = BuildAnalyticSummaryQuery(input, tempTableName, groupFieldsConfig, measureFieldsConfig);
                ExecuteNonQueryText(query, (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@FromDate", input.Query.FromTime));
                    cmd.Parameters.Add(new SqlParameter("@ToDate", input.Query.ToTime));
                });
            };

            Dictionary<string, string> columnsMappings = new Dictionary<string, string>();
            for (int i = 0; i < input.Query.GroupFields.Length; i++ )
            {
                var groupField = input.Query.GroupFields[i];
                columnsMappings.Add(String.Format("GroupFieldValues[{0}].Name", (int)groupField), String.Format("GroupFieldName_{0}", groupField));
            }

            for (int i = 0; i < input.Query.MeasureFields.Length; i++)
            {
                var measureField = input.Query.MeasureFields[i];
                columnsMappings.Add(String.Format("MeasureValues[{0}]", (int)measureField), String.Format("Measure_{0}", measureField));
            }

            return RetrieveData(input, createTempTableIfNotExistsAction, (reader) => AnalyticRecordMapper(reader, groupFieldsConfig, measureFieldsConfig), columnsMappings);
        }

        private string BuildAnalyticSummaryQuery(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input, string tempTableName, Dictionary<AnalyticGroupField, AnalyticGroupFieldConfig> groupFieldsConfig, Dictionary<AnalyticMeasureField, AnalyticMeasureFieldConfig> measureFieldsConfig)
        {
            StringBuilder queryBuilder = new StringBuilder(@"IF NOT OBJECT_ID('#TEMPTABLE#', N'U') IS NOT NULL
	                                                        BEGIN 
			                                                    SELECT #SELECTPART#
                                                                INTO #TEMPTABLE#
			                                                    FROM TrafficStats ts
                                                                #JOINPART#
			                                                    WHERE
			                                                    FirstCDRAttempt BETWEEN @FromDate AND @ToDate
                                                                #FILTERPART#
			                                                    GROUP BY #GROUPBYPART# ,cust.NameSuffix , custProf.Name
                                                            END ");
            
           
            StringBuilder selectPartBuilder = new StringBuilder();
            StringBuilder joinPartBuilder = new StringBuilder();
            StringBuilder filterPartBuilder = new StringBuilder();
            StringBuilder groupByPartBuilder = new StringBuilder();

            //adding group fields related parts to the query
            foreach (AnalyticGroupField groupField in input.Query.GroupFields)
            {                
                AnalyticGroupFieldConfig groupFieldConfig = groupFieldsConfig[groupField];

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
                
                if(!String.IsNullOrEmpty(groupFieldConfig.IdColumn))
                    AddColumnToSelectPart(selectPartBuilder, String.Format("{0} AS GroupFieldId_{1}", groupFieldConfig.IdColumn, groupField));
                
                if (!String.IsNullOrEmpty(groupFieldConfig.NameColumn))
                    AddColumnToSelectPart(selectPartBuilder, String.Format("{0} AS GroupFieldName_{1}", groupFieldConfig.NameColumn, groupField));
            }

            //adding Measures related parts to the query
            foreach (AnalyticMeasureField measureField in input.Query.MeasureFields)
            {
                AnalyticMeasureFieldConfig measureFieldConfig = measureFieldsConfig[measureField];

                AddColumnToSelectPart(selectPartBuilder, String.Format("{0} AS Measure_{1}", measureFieldConfig.GetFieldExpression(input.Query), measureField));
            }

            queryBuilder.Replace("#TEMPTABLE#", tempTableName);
            queryBuilder.Replace("#SELECTPART#", selectPartBuilder.ToString());
            queryBuilder.Replace("#JOINPART#", joinPartBuilder.ToString());
            queryBuilder.Replace("#FILTERPART#", filterPartBuilder.ToString());
            queryBuilder.Replace("#GROUPBYPART#", groupByPartBuilder.ToString());

            return queryBuilder.ToString();
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

        void AddStatementToJoinPart(StringBuilder joinPartBuilder, string statement)
        {
            joinPartBuilder.Append(statement);
        }

        void AddFilterToFilterPart(StringBuilder filterPartBuilder, string filter)
        {
            filterPartBuilder.Append(filter);
        }

        AnalyticRecord AnalyticRecordMapper(IDataReader reader, Dictionary<AnalyticGroupField, AnalyticGroupFieldConfig> groupFieldsConfig, Dictionary<AnalyticMeasureField, AnalyticMeasureFieldConfig> measureFieldsConfig)
        {
            AnalyticRecord record = new AnalyticRecord()
            {
                GroupFieldValues = new AnalyticGroupFieldValue[groupFieldsConfig.Count],
                MeasureValues = new Object[measureFieldsConfig.Count]
            };

            var index = 0;
            foreach (var groupFieldConfig in groupFieldsConfig)
            {
                object id = reader[String.Format("GroupFieldId_{0}", groupFieldConfig.Key)];
                if (id != DBNull.Value)
                {
                    string nameColumnName = String.Format("GroupFieldName_{0}", groupFieldConfig.Key);
                    record.GroupFieldValues[index] = new AnalyticGroupFieldValue
                    {
                        Id = id.ToString(),
                        Name = reader[nameColumnName] as string
                    };
                }

                index++;
            }

            index = 0;

            foreach (var measureFieldConfig in measureFieldsConfig)
            {

                string columnName = String.Format("Measure_{0}", measureFieldConfig.Key);
                record.MeasureValues[index] = GetReaderValue<object>(reader, columnName);

                index++;
            }

            return record;
        }
    }
}
