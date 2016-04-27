using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.SQLDataStorage
{
    public class SQLRecordStorageDataManager : BaseSQLDataManager, IDataRecordDataManager, ISummaryRecordDataManager
    {
        SQLDataStoreSettings _dataStoreSettings;
        SQLDataRecordStorageSettings _dataRecordStorageSettings;
        DataRecordStorage _dataRecordStorage;
        SummaryTransformationDefinition _summaryTransformationDefinition;

        DataRecordType _dataRecordType;
        DataRecordType DataRecordType
        {
            get
            {
                if (_dataRecordType == null)
                {
                    DataRecordTypeManager manager = new DataRecordTypeManager();
                    _dataRecordType = manager.GetDataRecordType(_dataRecordStorage.DataRecordTypeId);
                    if (_dataRecordType == null)
                        throw new NullReferenceException(String.Format("_dataRecordType ID '{0}'", _dataRecordStorage.DataRecordTypeId));
                }
                return _dataRecordType;
            }
        }

        internal SQLRecordStorageDataManager(SQLDataStoreSettings dataStoreSettings, SQLDataRecordStorageSettings dataRecordStorageSettings, DataRecordStorage dataRecordStorage)
            : base(dataStoreSettings.ConnectionString, false)
        {
            this._dataStoreSettings = dataStoreSettings;
            this._dataRecordStorageSettings = dataRecordStorageSettings;
            this._dataRecordStorage = dataRecordStorage;
        }

        internal SQLRecordStorageDataManager(SQLDataStoreSettings dataStoreSettings, SQLDataRecordStorageSettings dataRecordStorageSettings, DataRecordStorage dataRecordStorage, SummaryTransformationDefinition summaryTransformationDefinition)
            : this(dataStoreSettings, dataRecordStorageSettings, dataRecordStorage)
        {
            this._summaryTransformationDefinition = summaryTransformationDefinition;
        }

        public void ApplyStreamToDB(object stream)
        {
            base.InsertBulkToTable(stream as BaseBulkInsertInfo);
        }

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        IDynamicManager _dynamicManager;
        IDynamicManager DynamicManager
        {
            get
            {
                if (_dynamicManager == null)
                {
                    if (_dataRecordStorage == null)
                        throw new NullReferenceException("_dataRecordStorage");
                    DynamicTypeGenerator dynamicTypeGenerator = new DynamicTypeGenerator();
                    _dynamicManager = dynamicTypeGenerator.GetDynamicManager(_dataRecordStorage, _dataRecordStorageSettings);
                    if (_dynamicManager == null)
                        throw new NullReferenceException("_dynamicManager");
                }
                return _dynamicManager;
            }
        }

        public void WriteRecordToStream(object record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            this.DynamicManager.WriteRecordToStream(record, streamForBulkInsert);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            string tableName = GetTableName();
            return new StreamBulkInsertInfo
            {
                TableName = tableName,
                Stream = streamForBulkInsert,
                ColumnNames = this.DynamicManager.ColumnNames,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }

        private string GetTableName()
        {
            string tableName = this._dataRecordStorageSettings.TableName;
            if (!String.IsNullOrEmpty(this._dataRecordStorageSettings.TableSchema))
                tableName = String.Format("{0}.{1}", this._dataRecordStorageSettings.TableSchema, this._dataRecordStorageSettings.TableName);
            return tableName;
        }

        public void CreateSQLRecordStorageTable()
        {
            ExecuteNonQueryText(GetRecordStorageCreateTableQuery(), null);
        }

        public void AlterSQLRecordStorageTable(SQLDataRecordStorageSettings existingDataRecordSettings)
        {
            string query = GetRecordStorageAlterTableQuery(existingDataRecordSettings);
            if (query != null)
                ExecuteNonQueryText(query, null);
        }

        #region Private Methods

        string GetRecordStorageCreateTableQuery()
        {
            StringBuilder query = new StringBuilder();
            string schema = (_dataRecordStorageSettings.TableSchema != null) ? _dataRecordStorageSettings.TableSchema : "dbo";
            string name = _dataRecordStorageSettings.TableName;

            if (schema != null)
                query.Append(String.Format(" IF NOT EXISTS (SELECT schema_name FROM information_schema.schemata WHERE schema_name = '{0}') BEGIN EXEC sp_executesql N'CREATE SCHEMA {0}' END;", schema));

            string tableName = (schema != null) ? String.Format("{0}.{1}", schema, name) : name;
            query.AppendLine(String.Format("CREATE TABLE {0}", tableName));

            query.AppendLine(BuildColumnDefinitionsScript());

            query.AppendLine(BuildDropAndCreateTableTypeScript());
            return query.ToString();
        }

        private string BuildColumnDefinitionsScript()
        {
            List<string> columnDefinitions = new List<string>();

            foreach (var column in _dataRecordStorageSettings.Columns)
            {
                columnDefinitions.Add(String.Format("{0} {1}", column.ColumnName, column.SQLDataType));
            }

            return String.Format(" ({0});", string.Join(",", columnDefinitions));
        }

        string BuildDropAndCreateTableTypeScript()
        {
            string schemaName = !String.IsNullOrEmpty(_dataRecordStorageSettings.TableSchema) ? _dataRecordStorageSettings.TableSchema : "dbo";
            string tableName = _dataRecordStorageSettings.TableName;
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(@"IF  EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N'{1}Type'
                                     AND ss.name = N'{0}')
                                    DROP TYPE [{0}].[{1}Type]
                                    
                                    "
                , schemaName, tableName);
            builder.AppendLine();
            builder.AppendFormat(@"CREATE TYPE [{0}].[{1}Type] AS TABLE ", schemaName, tableName);
            builder.AppendLine(BuildColumnDefinitionsScript());
            return builder.ToString();
        }

        string GetRecordStorageAlterTableQuery(SQLDataRecordStorageSettings existingDataRecordSettings)
        {
            StringBuilder builder = new StringBuilder();

            string existingSchema = (existingDataRecordSettings.TableSchema != null) ? existingDataRecordSettings.TableSchema : "dbo";
            string existingName = existingDataRecordSettings.TableName;
            string newSchema = (_dataRecordStorageSettings.TableSchema != null) ? _dataRecordStorageSettings.TableSchema : "dbo";
            string newName = _dataRecordStorageSettings.TableName;

            if (existingSchema.CompareTo(newSchema) != 0)
            {
                builder.Append(String.Format("IF NOT EXISTS (SELECT schema_name FROM information_schema.schemata WHERE schema_name = '{0}') BEGIN EXEC sp_executesql N'CREATE SCHEMA {0}' END;", newSchema));

                builder.Append(string.Format("ALTER SCHEMA {0} TRANSFER {1}.{2};", newSchema, existingSchema, existingName));
            }

            if (existingName.CompareTo(newName) != 0)
                builder.Append(String.Format("BEGIN EXEC sp_executesql N'sp_rename ''{0}.{1}'', ''{2}''' END;", newSchema, existingName, newName));

            List<string> addColumnDefinitions;
            List<string> alterColumnDefinitions;

            SetAddAndAlterColumnDefinitions(existingDataRecordSettings, out addColumnDefinitions, out alterColumnDefinitions);

            if (addColumnDefinitions.Count() > 0)
                builder.Append(String.Format("ALTER TABLE {0}.{1} ADD {2};", newSchema, newName, String.Join(",", addColumnDefinitions)));

            foreach (var columnDefinition in alterColumnDefinitions)
            {
                builder.Append(String.Format("ALTER TABLE {0}.{1} ALTER COLUMN {2};", newSchema, newName, columnDefinition));
            }

            builder.AppendLine(BuildDropAndCreateTableTypeScript());

            return builder.ToString();
        }

        void SetAddAndAlterColumnDefinitions(SQLDataRecordStorageSettings existingDataRecordSettings, out List<string> addColumnDefinitions, out List<string> alterColumnDefinitions)
        {
            addColumnDefinitions = new List<string>();
            alterColumnDefinitions = new List<string>();

            foreach (var column in _dataRecordStorageSettings.Columns)
            {
                var existingMatch = existingDataRecordSettings.Columns.FirstOrDefault(itm => itm.ColumnName == column.ColumnName);
                if (existingMatch == null)
                    addColumnDefinitions.Add(String.Format("{0} {1}", column.ColumnName, column.SQLDataType));
                else if (column.SQLDataType != existingMatch.SQLDataType)
                    alterColumnDefinitions.Add(String.Format("{0} {1}", column.ColumnName, column.SQLDataType));
            }
        }

        void AppendErrorCheck(StringBuilder sql)
        {
            sql.Append(" IF @@ERROR <> 0 BEGIN ROLLBACK TRAN END;");
        }

        #endregion

        public void InsertSummaryRecords(IEnumerable<dynamic> records)
        {
            var dbApplyStream = this.InitialiazeStreamForDBApply();
            foreach (var record in records)
            {
                this.WriteRecordToStream(record, dbApplyStream);
            }
            var readyStream = this.FinishDBApplyStream(dbApplyStream);
            this.ApplyStreamToDB(readyStream);
        }

        public void UpdateSummaryRecords(IEnumerable<dynamic> records)
        {
            StringBuilder queryBuilder = new StringBuilder(@"UPDATE existingRecord
                                SET #COLUMNSUPDATE#
                                FROM #TABLE# existingRecord JOIN @UpdatedRecords updatedRecord ON existingRecord.#IDCOLUMN# = updatedRecord.#IDCOLUMN#");
            var idColumnMapping = _dataRecordStorageSettings.Columns.FirstOrDefault(itm => itm.ValueExpression == _summaryTransformationDefinition.SummaryIdFieldName);
            if (idColumnMapping == null)
                throw new NullReferenceException(String.Format("idColumnMapping '{0}'", _summaryTransformationDefinition.SummaryBatchStartFieldName));
            if (idColumnMapping.ColumnName == null)
                throw new NullReferenceException(String.Format("idColumnMapping.ColumnName '{0}'", _summaryTransformationDefinition.SummaryBatchStartFieldName));

            StringBuilder columnsUpdateBuilder = new StringBuilder();
            foreach (var columnsMapping in _dataRecordStorageSettings.Columns)
            {
                if (columnsUpdateBuilder.Length > 0)
                    columnsUpdateBuilder.Append(", ");
                columnsUpdateBuilder.AppendLine(String.Format("{0} = updatedRecord.{0}", columnsMapping.ColumnName));
            }

            queryBuilder.Replace("#TABLE#", GetTableName());
            queryBuilder.Replace("#IDCOLUMN#", idColumnMapping.ColumnName);
            queryBuilder.Replace("#COLUMNSUPDATE#", columnsUpdateBuilder.ToString());

            DataTable dt = this.DynamicManager.ConvertDataRecordsToTable(records);
            ExecuteNonQueryText(queryBuilder.ToString(),
                (cmd) =>
                {
                    SqlParameter prm = new SqlParameter("@UpdatedRecords", System.Data.SqlDbType.Structured);
                    prm.TypeName = String.Format("{0}Type", GetTableName());
                    prm.Value = dt;
                    cmd.Parameters.Add(prm);
                });
        }

        public IEnumerable<dynamic> GetExistingSummaryRecords(DateTime batchStart)
        {
            var recortTypeManager = new DataRecordTypeManager();
            var recordRuntimeType = recortTypeManager.GetDataRecordRuntimeType(_dataRecordStorage.DataRecordTypeId);
            if (recordRuntimeType == null)
                throw new NullReferenceException(String.Format("recordRuntimeType '{0}'", _dataRecordStorage.DataRecordTypeId));
            StringBuilder queryBuilder = new StringBuilder(@"SELECT #COLUMNS# FROM #TABLE# WHERE #BATCHSTARTCOLUMN# = @BatchStart");

            var batchStartColumnMapping = _dataRecordStorageSettings.Columns.FirstOrDefault(itm => itm.ValueExpression == _summaryTransformationDefinition.SummaryBatchStartFieldName);
            if (batchStartColumnMapping == null)
                throw new NullReferenceException(String.Format("batchStartColumnMapping '{0}'", _summaryTransformationDefinition.SummaryBatchStartFieldName));
            if (batchStartColumnMapping.ColumnName == null)
                throw new NullReferenceException(String.Format("batchStartColumnMapping.ColumnName '{0}'", _summaryTransformationDefinition.SummaryBatchStartFieldName));

            queryBuilder.Replace("#COLUMNS#", this.DynamicManager.ColumnNamesCommaDelimited);
            queryBuilder.Replace("#TABLE#", GetTableName());
            queryBuilder.Replace("#BATCHSTARTCOLUMN#", batchStartColumnMapping.ColumnName);
            return GetItemsText(queryBuilder.ToString(),
                (reader) =>
                {
                    dynamic item = Activator.CreateInstance(recordRuntimeType) as dynamic;
                    this.DynamicManager.FillDataRecordFromReader(item, reader);
                    return item;
                },
                (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@BatchStart", batchStart));
                });
        }

        public Vanrise.Entities.BigResult<DataRecord> GetFilteredDataRecords(Vanrise.Entities.DataRetrievalInput<DataRecordQuery> input, out List<DataRecordColumn> columns)
        {
            columns = BuildColumnsDefinitions();
            Action<string> createTempTableAction = (tempTableName) =>
                {

                    int parameterIndex = 0;
                    Dictionary<string, Object> parameterValues = new Dictionary<string, object>();
                    String recordFilter = BuildRecordFilter(input.Query.FilterGroup, ref parameterIndex, parameterValues);
                    string query = BuildQuery(input, tempTableName, recordFilter);
                    ExecuteNonQueryText(query,
                        (cmd) =>
                        {
                            cmd.Parameters.Add(new SqlParameter("@FromTime", ToDBNullIfDefault(input.Query.FromTime)));
                            cmd.Parameters.Add(new SqlParameter("@ToTime", ToDBNullIfDefault(input.Query.ToTime)));
                            foreach (var prm in parameterValues)
                            {
                                cmd.Parameters.Add(new SqlParameter(prm.Key, prm.Value));
                            }
                        });
                };
            return RetrieveData(input, createTempTableAction, DataRecordMapper);
        }

        private List<DataRecordColumn> BuildColumnsDefinitions()
        {
            List<DataRecordColumn> columnDefinitions = new List<DataRecordColumn>();

            foreach (var column in _dataRecordStorageSettings.Columns)
            {
                DataRecordColumn item = new DataRecordColumn()
                {
                    Title = column.ValueExpression
                };
                columnDefinitions.Add(item);
            }

            return columnDefinitions;
        }

        private string BuildQuery(Vanrise.Entities.DataRetrievalInput<DataRecordQuery> input, string tempTableName, string recordFilter)
        {
            string dateTimeColumn = GetColumnNameFromFieldName(_dataRecordStorageSettings.DateTimeField);
            string tableName = GetTableName();

            string recordFilterResult = !string.IsNullOrEmpty(recordFilter) ? string.Format(" and {0} ", recordFilter) : string.Empty;
            StringBuilder str = new StringBuilder(string.Format(@"IF OBJECT_ID('{0}', N'U') IS NULL 
                                                                BEGIN 
                                                                    select * INTO {0} from {1} 
                                                                    where (@FromTime is null or {2} >= @FromTime) 
                                                                    and (@ToTime is null or {2} < @ToTime) 
                                                                    {3}
                                                                END", tempTableName, tableName, dateTimeColumn, recordFilterResult));
            input.SortByColumnName = dateTimeColumn;
            return str.ToString();
        }

        private DataRecord DataRecordMapper(IDataReader reader)
        {
            Dictionary<string, Object> fieldValues = new Dictionary<string, object>();
            foreach (var column in _dataRecordStorageSettings.Columns)
            {
                fieldValues.Add(column.ValueExpression, reader[column.ColumnName]);
            }
            return new DataRecord { FieldValues = fieldValues };
        }

        private string BuildRecordFilter(RecordFilterGroup filterGroup, ref int parameterIndex, Dictionary<string, Object> parameterValues)
        {
            if (filterGroup == null || filterGroup.Filters == null)
                return null;

            StringBuilder builder = new StringBuilder();
            foreach (var filter in filterGroup.Filters)
            {
                if (builder.Length > 0)
                    builder.AppendFormat(" {0} ", filterGroup.LogicalOperator);

                RecordFilterGroup childFilterGroup = filter as RecordFilterGroup;
                if (childFilterGroup != null)
                {
                    builder.Append(BuildRecordFilter(childFilterGroup, ref parameterIndex, parameterValues));
                    continue;
                }

                EmptyRecordFilter emptyFilter = filter as EmptyRecordFilter;
                if (emptyFilter != null)
                {
                    builder.Append(BuildRecordFilter(emptyFilter, ref parameterIndex, parameterValues));
                    continue;
                }

                NonEmptyRecordFilter nonEmptyFilter = filter as NonEmptyRecordFilter;
                if (nonEmptyFilter != null)
                {
                    builder.Append(BuildRecordFilter(nonEmptyFilter, ref parameterIndex, parameterValues));
                    continue;
                }

                StringRecordFilter stringFilter = filter as StringRecordFilter;
                if (stringFilter != null)
                {
                    builder.Append(BuildRecordFilter(stringFilter, ref parameterIndex, parameterValues));
                    continue;
                }

                NumberRecordFilter numberFilter = filter as NumberRecordFilter;
                if (numberFilter != null)
                {
                    builder.Append(BuildRecordFilter(numberFilter, ref parameterIndex, parameterValues));
                    continue;
                }

                DateTimeRecordFilter dateTimeFilter = filter as DateTimeRecordFilter;
                if (dateTimeFilter != null)
                {
                    builder.Append(BuildRecordFilter(dateTimeFilter, ref parameterIndex, parameterValues));
                    continue;
                }

                BooleanRecordFilter booleanFilter = filter as BooleanRecordFilter;
                if (booleanFilter != null)
                {
                    builder.Append(BuildRecordFilter(booleanFilter, ref parameterIndex, parameterValues));
                    continue;
                }

                NumberListRecordFilter numberListFilter = filter as NumberListRecordFilter;
                if (numberListFilter != null)
                {
                    builder.Append(BuildRecordFilter(numberListFilter, ref parameterIndex, parameterValues));
                    continue;
                }
            }

            return String.Format("({0})", builder);
        }

        private string BuildRecordFilter(EmptyRecordFilter emptyFilter, ref int parameterIndex, Dictionary<string, Object> parameterValues)
        {
            return string.Format("{0} IS NULL", GetColumnName(emptyFilter));
        }

        private string BuildRecordFilter(NonEmptyRecordFilter nonEmptyFilter, ref int parameterIndex, Dictionary<string, Object> parameterValues)
        {
            return string.Format("{0} IS NOT NULL", GetColumnName(nonEmptyFilter));
        }

        private string BuildRecordFilter(StringRecordFilter stringFilter, ref int parameterIndex, Dictionary<string, Object> parameterValues)
        {
            string parameterName = GenerateParameterName(ref parameterIndex);
            string modifiedParameterName = parameterName;
            string compareOperator = null;

            switch (stringFilter.CompareOperator)
            {
                case StringRecordFilterOperator.Equals: compareOperator = "="; break;
                case StringRecordFilterOperator.NotEquals: compareOperator = "<>"; break;
                case StringRecordFilterOperator.Contains:
                    compareOperator = "LIKE";
                    modifiedParameterName = string.Format("'%' + {0} + '%'", parameterName);
                    break;
                case StringRecordFilterOperator.NotContains:
                    compareOperator = "NOT LIKE";
                    modifiedParameterName = string.Format("'%' + {0} + '%'", parameterName);
                    break;
                case StringRecordFilterOperator.StartsWith:
                    compareOperator = "LIKE";
                    modifiedParameterName = string.Format("{0} + '%'", parameterName);
                    break;
                case StringRecordFilterOperator.NotStartsWith:
                    compareOperator = "NOT LIKE";
                    modifiedParameterName = string.Format("{0} + '%'", parameterName);
                    break;
                case StringRecordFilterOperator.EndsWith:
                    compareOperator = "LIKE";
                    modifiedParameterName = string.Format("'%' + {0}", parameterName);
                    break;
                case StringRecordFilterOperator.NotEndsWith:
                    compareOperator = "NOT LIKE";
                    modifiedParameterName = string.Format("'%' + {0}", parameterName);
                    break;
            }
            parameterValues.Add(parameterName, stringFilter.Value);
            return string.Format("{0} {1} {2}", GetColumnName(stringFilter), compareOperator, modifiedParameterName);
        }

        private string BuildRecordFilter(NumberRecordFilter numberFilter, ref int parameterIndex, Dictionary<string, Object> parameterValues)
        {
            string parameterName = GenerateParameterName(ref parameterIndex);
            string compareOperator = null;

            switch (numberFilter.CompareOperator)
            {
                case NumberRecordFilterOperator.Equals: compareOperator = "="; break;
                case NumberRecordFilterOperator.NotEquals: compareOperator = "<>"; break;
                case NumberRecordFilterOperator.Greater: compareOperator = ">"; break;
                case NumberRecordFilterOperator.GreaterOrEquals: compareOperator = ">="; break;
                case NumberRecordFilterOperator.Less: compareOperator = "<"; break;
                case NumberRecordFilterOperator.LessOrEquals: compareOperator = "<="; break;
            }
            parameterValues.Add(parameterName, numberFilter.Value);
            return string.Format("{0} {1} {2}", GetColumnName(numberFilter), compareOperator, parameterName);
        }

        private string BuildRecordFilter(DateTimeRecordFilter dateTimeFilter, ref int parameterIndex, Dictionary<string, Object> parameterValues)
        {
            string parameterName = GenerateParameterName(ref parameterIndex);
            string compareOperator = null;

            switch (dateTimeFilter.CompareOperator)
            {
                case DateTimeRecordFilterOperator.Equals: compareOperator = "="; break;
                case DateTimeRecordFilterOperator.NotEquals: compareOperator = "<>"; break;
                case DateTimeRecordFilterOperator.Greater: compareOperator = ">"; break;
                case DateTimeRecordFilterOperator.GreaterOrEquals: compareOperator = ">="; break;
                case DateTimeRecordFilterOperator.Less: compareOperator = "<"; break;
                case DateTimeRecordFilterOperator.LessOrEquals: compareOperator = "<="; break;
            }
            parameterValues.Add(parameterName, dateTimeFilter.Value);
            return string.Format("{0} {1} {2}", GetColumnName(dateTimeFilter), compareOperator, parameterName);
        }

        private string BuildRecordFilter(BooleanRecordFilter booleanFilter, ref int parameterIndex, Dictionary<string, Object> parameterValues)
        {
            return string.Format("{0} = {1}", GetColumnName(booleanFilter), booleanFilter.IsTrue ? "1" : "0");
        }

        private string BuildRecordFilter(NumberListRecordFilter numberListFilter, ref int parameterIndex, Dictionary<string, Object> parameterValues)
        {
            return BuildRecordFilter<Decimal>(numberListFilter, ref parameterIndex, parameterValues);
        }

        string BuildRecordFilter<T>(ListRecordFilter<T> listFilter, ref int parameterIndex, Dictionary<string, Object> parameterValues)
        {
            StringBuilder valuesBuilder = new StringBuilder();
            foreach (var value in listFilter.Values)
            {
                if (valuesBuilder.Length > 0)
                    valuesBuilder.Append(", ");
                string parameterName = GenerateParameterName(ref parameterIndex);
                parameterValues.Add(parameterName, value);
                valuesBuilder.Append(parameterName);
            }
            string compareOperator = listFilter.CompareOperator == ListRecordFilterOperator.In ? "IN" : "NOT IN";
            return string.Format("{0} {1} ({2})", GetColumnName(listFilter), compareOperator, valuesBuilder);
        }

        string GenerateParameterName(ref int parameterIndex)
        {
            return String.Format("@Prm_{0}", parameterIndex++);
        }

        string GetColumnName(RecordFilter recordFilter)
        {
            return GetColumnNameFromFieldName(recordFilter.FieldName);
        }

        string GetColumnNameFromFieldName(string fieldName)
        {
            var column = _dataRecordStorageSettings.Columns.FirstOrDefault(itm => itm.ValueExpression == fieldName);
            if (column == null)
                throw new NullReferenceException(String.Format("column. RecordStorageId '{0}'. FieldName '{1}'", -_dataRecordStorage.DataRecordStorageId, fieldName));
            return column.ColumnName;
        }
    }
}
