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


        List<string> Columns;
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
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
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

        protected override string GetConnectionString()
        {
            if (_dataStoreSettings == null)
                throw new NullReferenceException("_dataStoreSettings");
            return !String.IsNullOrEmpty(_dataStoreSettings.ConnectionString) ? _dataStoreSettings.ConnectionString : Common.Utilities.GetExposedConnectionString(_dataStoreSettings.ConnectionStringName);
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

        public List<DataRecord> GetFilteredDataRecords(Vanrise.Entities.DataRetrievalInput<DataRecordQuery> input)
        {
            Columns = GetColumnNamesFromFieldNames(input.Query.Columns);
            int parameterIndex = 0;
            Dictionary<string, Object> parameterValues = new Dictionary<string, object>();
            Data.SQL.RecordFilterSQLBuilder recordFilterSQLBuilder = new Data.SQL.RecordFilterSQLBuilder(GetColumnNameFromFieldName);
            String recordFilter = recordFilterSQLBuilder.BuildRecordFilter(input.Query.FilterGroup, ref parameterIndex, parameterValues);
            string query = BuildQuery(input, recordFilter);

            return GetItemsText(query, DataRecordMapper, (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@FromTime", ToDBNullIfDefault(input.Query.FromTime)));
                    cmd.Parameters.Add(new SqlParameter("@ToTime", ToDBNullIfDefault(input.Query.ToTime)));
                    foreach (var prm in parameterValues)
                    {
                        cmd.Parameters.Add(new SqlParameter(prm.Key, prm.Value));
                    }
                });

        }

        public void GetDataRecords(DateTime from, DateTime to, Action<dynamic> onItemReady)
        {
            var recortTypeManager = new DataRecordTypeManager();
            var recordRuntimeType = recortTypeManager.GetDataRecordRuntimeType(_dataRecordStorage.DataRecordTypeId);
            if (recordRuntimeType == null)
                throw new NullReferenceException(String.Format("recordRuntimeType '{0}'", _dataRecordStorage.DataRecordTypeId));

            string dateTimeColumn = GetColumnNameFromFieldName(_dataRecordStorageSettings.DateTimeField);
            string tableName = GetTableName();

            Dictionary<string, Object> parameterValues = new Dictionary<string, object>();


            string query = string.Format(@"select  * from {0} WITH (NOLOCK)
                                           where ({1} >= @FromTime) 
                                           and ({1} < @ToTime)", tableName, dateTimeColumn);
            ExecuteReaderText(query, (reader) =>
            {
                while (reader.Read())
                {
                    dynamic item = Activator.CreateInstance(recordRuntimeType) as dynamic;
                    this.DynamicManager.FillDataRecordFromReader(item, reader);
                    onItemReady(item);
                }
            }, (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@FromTime", from));
                cmd.Parameters.Add(new SqlParameter("@ToTime", to));
                foreach (var prm in parameterValues)
                {
                    cmd.Parameters.Add(new SqlParameter(prm.Key, prm.Value));
                }
            });
        }

        public void DeleteRecords(DateTime from, DateTime to)
        {
            string dateTimeColumn = GetColumnNameFromFieldName(_dataRecordStorageSettings.DateTimeField);
            string tableName = GetTableName();
            Dictionary<string, Object> parameterValues = new Dictionary<string, object>();

            string query = string.Format(@"delete {0} from {0} WITH (NOLOCK)
                                           where ({1} >= @FromTime) 
                                           and ({1} < @ToTime)", tableName, dateTimeColumn);


            ExecuteNonQueryText(query, (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@FromTime", from));
                cmd.Parameters.Add(new SqlParameter("@ToTime", to));
                foreach (var prm in parameterValues)
                {
                    cmd.Parameters.Add(new SqlParameter(prm.Key, prm.Value));
                }
            });
        }

        public void DeleteRecords(DateTime dateTime)
        {
            string dateTimeColumn = GetColumnNameFromFieldName(_dataRecordStorageSettings.DateTimeField);
            string tableName = GetTableName();
            Dictionary<string, Object> parameterValues = new Dictionary<string, object>();

            string query = string.Format(@"delete {0} from {0} WITH (NOLOCK)
                                           where ({1} = @dateTime)", tableName, dateTimeColumn);


            ExecuteNonQueryText(query, (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@dateTime", dateTime));
                foreach (var prm in parameterValues)
                {
                    cmd.Parameters.Add(new SqlParameter(prm.Key, prm.Value));
                }
            });
        }

        private string BuildQuery(Vanrise.Entities.DataRetrievalInput<DataRecordQuery> input, string recordFilter)
        {
            string dateTimeColumn = GetColumnNameFromFieldName(_dataRecordStorageSettings.DateTimeField);
            string tableName = GetTableName();

            string recordFilterResult = !string.IsNullOrEmpty(recordFilter) ? string.Format(" and {0} ", recordFilter) : string.Empty;
            string orderResult = string.Format(" Order By {0} {1} ", dateTimeColumn, input.Query.Direction == OrderDirection.Ascending ? "ASC" : "DESC");
            StringBuilder str = new StringBuilder(string.Format(@"  select Top {0} {1} from {2} WITH (NOLOCK)
                                                                    where ({3} >= @FromTime) 
                                                                    and (@ToTime is null or {3} <= @ToTime) 
                                                                    {4} {5}", input.Query.LimitResult, string.Join<string>(",", GetColumnNamesFromFieldNames(input.Query.Columns)), tableName, dateTimeColumn, recordFilterResult, orderResult));
            //input.SortByColumnName = dateTimeColumn;
            return str.ToString();
        }

        private DataRecord DataRecordMapper(IDataReader reader)
        {
            DateTime recordTime = default(DateTime);
            string dateTimeColumn = GetColumnNameFromFieldName(_dataRecordStorageSettings.DateTimeField);

            Dictionary<string, Object> fieldValues = new Dictionary<string, object>();
            foreach (var column in _dataRecordStorageSettings.Columns)
            {
                if (Columns.Contains(column.ColumnName))
                {
                    var value = reader[column.ColumnName];
                    if (value == DBNull.Value)
                        value = null;
                    if (string.Compare(dateTimeColumn, column.ColumnName, true) == 0)
                    {
                        recordTime = value != null ? (DateTime)value : default(DateTime);
                    }
                    fieldValues.Add(column.ValueExpression, value);
                }
            }
            return new DataRecord { RecordTime = recordTime, FieldValues = fieldValues };
        }

        string GetColumnNameFromFieldName(string fieldName)
        {
            var column = _dataRecordStorageSettings.Columns.FirstOrDefault(itm => itm.ValueExpression == fieldName);
            if (column == null)
                throw new NullReferenceException(String.Format("column. RecordStorageId '{0}'. FieldName '{1}'", _dataRecordStorage.DataRecordStorageId, fieldName));
            return column.ColumnName;
        }

        List<string> GetColumnNamesFromFieldNames(List<string> fieldNames)
        {
            if (fieldNames == null || fieldNames.Count == 0)
                return null;
            List<string> result = new List<string>();
            foreach (string fieldName in fieldNames)
            {
                result.Add(GetColumnNameFromFieldName(fieldName));
            }
            return result;
        }
    }
}