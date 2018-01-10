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
using Vanrise.Common;
using System.Data.Common;

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
        SQLTempStorageInformation _sqlTempStorageInformation;

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

        internal SQLRecordStorageDataManager(SQLDataStoreSettings dataStoreSettings, SQLDataRecordStorageSettings dataRecordStorageSettings, DataRecordStorage dataRecordStorage, SQLTempStorageInformation sqlTempStorageInformation)
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {
            this._dataStoreSettings = dataStoreSettings;
            this._dataRecordStorageSettings = dataRecordStorageSettings;
            this._dataRecordStorage = dataRecordStorage;
            this._sqlTempStorageInformation = sqlTempStorageInformation;
        }

        internal SQLRecordStorageDataManager(SQLDataStoreSettings dataStoreSettings, SQLDataRecordStorageSettings dataRecordStorageSettings, DataRecordStorage dataRecordStorage, SummaryTransformationDefinition summaryTransformationDefinition, SQLTempStorageInformation sqlTempStorageInformation)
            : this(dataStoreSettings, dataRecordStorageSettings, dataRecordStorage, sqlTempStorageInformation)
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
            string tableName = GetTableNameWithSchema();
            return new StreamBulkInsertInfo
            {
                TableName = _sqlTempStorageInformation != null ? _sqlTempStorageInformation.TableNameWithSchema : tableName,
                Stream = streamForBulkInsert,
                ColumnNames = this.DynamicManager.ColumnNames,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }

        private string GetTableNameWithSchema()
        {
            string tableName = this._dataRecordStorageSettings.TableName;
            if (!String.IsNullOrEmpty(this._dataRecordStorageSettings.TableSchema))
                tableName = String.Format("{0}.{1}", this._dataRecordStorageSettings.TableSchema, this._dataRecordStorageSettings.TableName);
            return tableName;
        }

        public void CreateSQLRecordStorageTable()
        {
            ExecuteNonQueryText(GetRecordStorageCreateTableQuery(this._dataRecordStorageSettings.TableName, false), null);
        }

        internal SQLTempStorageInformation CreateTempSQLRecordStorageTable(long processId)
        {
            string tableName = string.Format("{0}_Temp_{1}", _dataRecordStorageSettings.TableName, processId);
            ExecuteNonQueryText(GetRecordStorageCreateTableQuery(tableName, true), null);

            return new SQLTempStorageInformation()
            {
                Schema = _dataRecordStorageSettings.TableSchema != null ? _dataRecordStorageSettings.TableSchema : "dbo",
                TableName = tableName
            };
        }

        public void AlterSQLRecordStorageTable(SQLDataRecordStorageSettings existingDataRecordSettings)
        {
            string query = GetRecordStorageAlterTableQuery(existingDataRecordSettings);
            if (query != null)
                ExecuteNonQueryText(query, null);
        }

        public void FillDataRecordStorageFromTempStorage(DateTime from, DateTime to)
        {
            _sqlTempStorageInformation.ThrowIfNull("_sqlTempStorageInformation");

            StringBuilder queryBuilder = new StringBuilder
            (
                @"IF OBJECT_ID('#destinationTableName#', N'U') IS NOT NULL and OBJECT_ID('#sourceTableName#', N'U') IS NOT NULL
                        Begin
	                        Begin Try
		                        Begin Transaction
			                        Delete from #destinationTableName#
			                        Where #dateTimeColumn# >= @FromTime and #dateTimeColumn# < @ToTime

			                        Insert into #destinationTableName# (#columns#)
			                        Select #columns# from #sourceTableName# WITH(NOLOCK) 
			                        Where #dateTimeColumn# >= @FromTime and #dateTimeColumn# < @ToTime
		                        Commit Transaction
	                        End Try
	                        Begin CATCH
		                        declare @ErrorMessage nvarchar(max), @ErrorSeverity int, @ErrorState int;
                                select @ErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
                                rollback Transaction;
                                raiserror (@ErrorMessage, @ErrorSeverity, @ErrorState);
	                        End CATCH
                        End"
            );

            string columns = string.Join(",", _dataRecordStorageSettings.Columns.Select(itm => itm.ColumnName));

            string destinationTableName = GetTableNameWithSchema();
            string sourceTableName = _sqlTempStorageInformation.TableNameWithSchema;
            string dateTimeColumn = GetColumnNameFromFieldName(_dataRecordStorageSettings.DateTimeField);
            string fromParam = string.Format("'{0}'", from.ToString());
            string toParam = string.Format("'{0}'", to.ToString());

            queryBuilder.Replace("#destinationTableName#", destinationTableName);
            queryBuilder.Replace("#sourceTableName#", _sqlTempStorageInformation.TableNameWithSchema);
            queryBuilder.Replace("#columns#", columns);
            queryBuilder.Replace("#dateTimeColumn#", dateTimeColumn.ToString());

            ExecuteNonQueryText(queryBuilder.ToString(), (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@FromTime", from));
                cmd.Parameters.Add(new SqlParameter("@ToTime", to));
            });
        }

        public int GetStorageRowCount()
        {
            StringBuilder queryBuilder = new StringBuilder
            (
                @"IF OBJECT_ID('#tableNameWithSchema#', N'U') IS NOT NULL 
                                Begin
                                    SELECT CAST(p.rows AS int)
                                    FROM sys.tables AS tbl
                                    INNER JOIN sys.indexes AS idx ON idx.object_id = tbl.object_id and idx.index_id < 2
                                    INNER JOIN sys.partitions AS p ON p.object_id = CAST(tbl.object_id AS int) and p.index_id = idx.index_id
                                    WHERE ((tbl.name=N'#tableName#' AND SCHEMA_NAME(tbl.schema_id)=N'#schema#'))
                                End"
            );

            string tableNameWithSchema, schema, tableName;

            if (_sqlTempStorageInformation != null && _sqlTempStorageInformation.TableNameWithSchema != null)
            {
                tableNameWithSchema = _sqlTempStorageInformation.TableNameWithSchema;
                schema = _sqlTempStorageInformation.Schema;
                tableName = _sqlTempStorageInformation.TableName;
            }
            else
            {
                tableNameWithSchema = GetTableNameWithSchema();
                schema = (_dataRecordStorageSettings.TableSchema != null) ? _dataRecordStorageSettings.TableSchema : "dbo";
                tableName = _dataRecordStorageSettings.TableName;
            }

            queryBuilder.Replace("#tableNameWithSchema#", tableNameWithSchema);
            queryBuilder.Replace("#schema#", schema);
            queryBuilder.Replace("#tableName#", tableName);

            return (int)ExecuteScalarText(queryBuilder.ToString(), null);
        }

        public void DropStorage()
        {
            StringBuilder queryBuilder = new StringBuilder
            (
                @"IF OBJECT_ID('#tableNameWithSchema#', N'U') IS NOT NULL 
                                Begin
                                    Drop Table #tableNameWithSchema#
                                End"
            );

            string tableNameWithSchema = null;
            if (_sqlTempStorageInformation != null)
                tableNameWithSchema = _sqlTempStorageInformation.TableNameWithSchema;
            else
                tableNameWithSchema = GetTableNameWithSchema();

            queryBuilder.Replace("#tableNameWithSchema#", tableNameWithSchema);

            ExecuteNonQueryText(queryBuilder.ToString(), null);
        }

        #region Private Methods

        string GetRecordStorageCreateTableQuery(string tableName, bool isTempTable)
        {
            StringBuilder query = new StringBuilder();

            string schema = (_dataRecordStorageSettings.TableSchema != null) ? _dataRecordStorageSettings.TableSchema : "dbo";
            if (schema != null)
                query.Append(String.Format(" IF NOT EXISTS (SELECT schema_name FROM information_schema.schemata WHERE schema_name = '{0}') BEGIN EXEC sp_executesql N'CREATE SCHEMA {0}' END;", schema));

            string tableNameWithSchema = (schema != null) ? String.Format("{0}.{1}", schema, tableName) : tableName;
            query.AppendLine(String.Format("CREATE TABLE {0}", tableNameWithSchema));
            query.AppendLine(BuildColumnDefinitionsScript());

            if (!isTempTable)
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
            if (_dataRecordStorageSettings.IncludeQueueItemId)
            {
                columnDefinitions.Add("QueueItemId bigint");
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

            if (_dataRecordStorageSettings.IncludeQueueItemId)
            {
                builder.Append(String.Format(@"if not exists (select column_name from INFORMATION_SCHEMA.columns
                                                              Where table_schema='{0}' AND table_name = '{1}' AND column_name = 'QueueItemId')
                                               ALTER TABLE {0}.{1} ADD QueueItemId bigint;", newSchema, newName));
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

            queryBuilder.Replace("#TABLE#", GetTableNameWithSchema());
            queryBuilder.Replace("#IDCOLUMN#", idColumnMapping.ColumnName);
            queryBuilder.Replace("#COLUMNSUPDATE#", columnsUpdateBuilder.ToString());

            DataTable dt = this.DynamicManager.ConvertDataRecordsToTable(records);
                ExecuteNonQueryText(queryBuilder.ToString(),
                    (cmd) =>
                    {
                        SqlParameter prm = new SqlParameter("@UpdatedRecords", System.Data.SqlDbType.Structured);
                        prm.TypeName = String.Format("{0}Type", GetTableNameWithSchema());
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
            queryBuilder.Replace("#TABLE#", GetTableNameWithSchema());
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
            string tableName = GetTableNameWithSchema();

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
            string tableName = GetTableNameWithSchema();
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
            string tableName = GetTableNameWithSchema();
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
            string tableName = GetTableNameWithSchema();

            string recordFilterResult = !string.IsNullOrEmpty(recordFilter) ? string.Format(" and {0} ", recordFilter) : string.Empty;
            string orderResult = string.Format(" Order By {0} {1} ", dateTimeColumn, input.Query.Direction == OrderDirection.Ascending ? "ASC" : "DESC");
            StringBuilder str = new StringBuilder(string.Format(@"  select Top {0} {1} from {2} WITH (NOLOCK)
                                                                    where (@FromTime is null or {3} >= @FromTime) 
                                                                    and (@ToTime is null or {3} <= @ToTime) {4} {5}",
                                                                    input.Query.LimitResult, string.Join<string>(",", GetColumnNamesFromFieldNames(input.Query.Columns)),
                                                                    tableName, dateTimeColumn, recordFilterResult, orderResult));
            //input.SortByColumnName = dateTimeColumn;
            return str.ToString();
        }
        string GenerateParameterName(ref int parameterIndex)
        {
            return String.Format("@Prm_{0}", parameterIndex++);
        }


        public bool Update(Dictionary<string, Object> fieldValues)
        {
            Dictionary<string, Object> parameterValues = new Dictionary<string, Object>();
            return ExecuteNonQueryText(BuildUpdateQuery(fieldValues, parameterValues), (cmd) =>
                    {
                        foreach (var prm in parameterValues)
                        {
                            cmd.Parameters.Add(new SqlParameter(String.Format("{0}", prm.Key), prm.Value));
                        }

                    }) > 0;

        }
        private string BuildUpdateQuery(Dictionary<string, Object> fieldValues, Dictionary<string, Object> parameterValues)
        {
            if (fieldValues == null || fieldValues.Count == 0)
                throw new Exception("fieldValues should not be null or empty.");
            StringBuilder queryBuilder = new StringBuilder();

            int parameterIndex = 0;
            string tableName = GetTableNameWithSchema();
            queryBuilder.Append(string.Format(@" update  {0} set   ", tableName));
            StringBuilder whereQuery = new StringBuilder();
            StringBuilder valuesQuery = new StringBuilder();

            foreach (var fieldValue in fieldValues)
            {
                var sqlDataRecordStorageColumn = GetColumnFromFieldName(fieldValue.Key);
                var parameter = GenerateParameterName(ref parameterIndex);
                parameterValues.Add(parameter, fieldValue.Value);
                if (sqlDataRecordStorageColumn.IsUnique)
                {
                    if (whereQuery.Length != 0)
                        whereQuery.Append(" AND ");
                    whereQuery.AppendFormat(@" {0} = {1}  ", sqlDataRecordStorageColumn.ColumnName, parameter);
                }
                else
                {
                    if (valuesQuery.Length != 0)
                        valuesQuery.Append(",");
                    valuesQuery.AppendFormat(@" {0} = {1}  ", sqlDataRecordStorageColumn.ColumnName, parameter);
                }
            }
            queryBuilder.Append(string.Format(@" {0} where {1}  ",valuesQuery.ToString(), whereQuery.ToString()));
            return queryBuilder.ToString();
        }

        public bool Insert(Dictionary<string, Object> fieldValues, out object insertedId)
        {
            Dictionary<string, Object> parameterValues = new Dictionary<string, Object>();
            bool withOutParameter = false;
            insertedId = null;
            SqlParameter sqlParameter = null;
            var effectedRows = ExecuteNonQueryText(BuildInsertQuery(fieldValues, parameterValues, ref withOutParameter), (cmd) =>
            {
                foreach (var prm in parameterValues)
                {
                    cmd.Parameters.Add(new SqlParameter(String.Format("{0}", prm.Key), prm.Value));
                }
                if (withOutParameter)
                {
                    sqlParameter = new SqlParameter("@Id", SqlDbType.BigInt);
                    sqlParameter.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(sqlParameter);
                }

            });
            if(withOutParameter)
                insertedId = sqlParameter.Value;
            return effectedRows > 0;
        }
        private string BuildInsertQuery(Dictionary<string, Object> fieldValues, Dictionary<string, Object> parameterValues,ref bool withOutParameter)
        {
            StringBuilder queryBuilder = new StringBuilder();
            if (fieldValues == null || fieldValues.Count == 0)
                throw new Exception("fieldValues should not be null or empty.");

            int parameterIndex = 0;
            string tableName = GetTableNameWithSchema();
            queryBuilder.Append(string.Format(@" insert into {0} ( ", tableName));
            StringBuilder valuesQuery = new StringBuilder();
            foreach (var fieldValue in fieldValues)
            {
                var sqlDataRecordStorageColumn = GetColumnFromFieldName(fieldValue.Key);
                var parameter = GenerateParameterName(ref parameterIndex);
                parameterValues.Add(parameter, fieldValue.Value);
                if (parameterIndex != 1)
                {
                    queryBuilder.Append(",");
                    valuesQuery.Append(",");
                }
                queryBuilder.AppendFormat(@" {0} ", sqlDataRecordStorageColumn.ColumnName);
                valuesQuery.AppendFormat(@" {0} ", parameter);
            }
           
            queryBuilder.Append(") ");
            queryBuilder.Append(string.Format(@" values ({0}) ", valuesQuery.ToString()));
            var idColumn = GetIdColumn();
            if (idColumn != null)
            {
                withOutParameter = true;
                queryBuilder.Append(" Set @Id = SCOPE_IDENTITY() ");
            }
            return queryBuilder.ToString();
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
                    DataRecordField matchingField = DataRecordType.Fields.FindRecord(itm => itm.Name == column.ValueExpression);
                    var value = reader[column.ColumnName];
                    if (value == DBNull.Value)
                        value = null;
                    if (string.Compare(dateTimeColumn, column.ColumnName, true) == 0)
                    {
                        recordTime = value != null ? (DateTime)value : default(DateTime);
                    }

                    if (value != null && matchingField.Type.StoreValueSerialized)
                    {
                        value = matchingField.Type.DeserializeValue(new DeserializeDataRecordFieldValueContext() { Value = value as string });
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
            {
                if (_dataRecordStorageSettings.NullableFields == null || !_dataRecordStorageSettings.NullableFields.Any(x => x.Name == fieldName))
                    throw new NullReferenceException(String.Format("column. RecordStorageId '{0}'. FieldName '{1}'", _dataRecordStorage.DataRecordStorageId, fieldName));

                return "null";
            }
            else
            {
                return column.ColumnName;
            }
        }
        SQLDataRecordStorageColumn GetColumnFromFieldName(string fieldName)
        {
            var column = _dataRecordStorageSettings.Columns.FirstOrDefault(itm => itm.ValueExpression == fieldName);
            if (column == null)
            {
                if (_dataRecordStorageSettings.NullableFields == null || !_dataRecordStorageSettings.NullableFields.Any(x => x.Name == fieldName))
                    throw new NullReferenceException(String.Format("column. RecordStorageId '{0}'. FieldName '{1}'", _dataRecordStorage.DataRecordStorageId, fieldName));

                return null;
            }
            else
            {
                return column;
            }
        }
        DataRecordField GetIdColumn()
        {
            var column = DataRecordType.Fields.FirstOrDefault(x => x.Name == DataRecordType.Settings.IdField);
            if (column == null)
            {
                return null;
            }
            else
            {
                return column;
            }
        }
        List<string> GetColumnNamesFromFieldNames(List<string> fieldNames)
        {
            if (fieldNames == null || fieldNames.Count == 0)
                return null;
            List<string> result = new List<string>();
            foreach (string fieldName in fieldNames)
            {
                string columnName = GetColumnNameFromFieldName(fieldName);
                if (string.Compare(columnName, "null", true) != 0)
                {
                    result.Add(columnName);
                }
            }
            return result;
        }
    }
}