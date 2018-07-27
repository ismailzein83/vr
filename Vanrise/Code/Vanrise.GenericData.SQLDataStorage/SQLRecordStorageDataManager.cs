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
        SQLTempStorageInformation _sqlTempStorageInformation;

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

        Dictionary<string, DataRecordField> _dataRecordFieldsByName;
        Dictionary<string, DataRecordField> DataRecordFieldsByName
        {
            get
            {
                if (_dataRecordFieldsByName == null)
                {
                    DataRecordTypeManager manager = new DataRecordTypeManager();
                    _dataRecordFieldsByName = manager.GetDataRecordTypeFields(_dataRecordStorage.DataRecordTypeId);
                    if (_dataRecordFieldsByName == null)
                        throw new NullReferenceException(String.Format("_dataRecordFieldsByName ID '{0}'", _dataRecordStorage.DataRecordTypeId));
                }
                return _dataRecordFieldsByName;
            }
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

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
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

        public void ApplyStreamToDB(object stream)
        {
            base.InsertBulkToTable(stream as BaseBulkInsertInfo);
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

        public void FillDataRecordStorageFromTempStorage(DateTime from, DateTime to, RecordFilterGroup recordFilterGroup)
        {
            _sqlTempStorageInformation.ThrowIfNull("_sqlTempStorageInformation");

            Dictionary<string, Object> parameterValues = new Dictionary<string, object>();
            int parameterIndex = 0;

            string recordFilterResult = string.Empty;
            if (recordFilterGroup != null)
            {
                Data.SQL.RecordFilterSQLBuilder recordFilterSQLBuilder = new Data.SQL.RecordFilterSQLBuilder(GetColumnNameFromFieldName);
                string recordFilter = recordFilterSQLBuilder.BuildRecordFilter(recordFilterGroup, ref parameterIndex, parameterValues);
                recordFilterResult = !string.IsNullOrEmpty(recordFilter) ? string.Format(" AND {0} ", recordFilter) : string.Empty;
            }

            StringBuilder queryBuilder = new StringBuilder
            (
                @"IF OBJECT_ID('#destinationTableName#', N'U') IS NOT NULL and OBJECT_ID('#sourceTableName#', N'U') IS NOT NULL
                        Begin
	                        Begin Try
		                        Begin Transaction
			                        Delete from #destinationTableName#
			                        Where #dateTimeColumn# >= @FromTime and #dateTimeColumn# < @ToTime #RecordFilterResult#

			                        Insert into #destinationTableName# (#columns#)
			                        Select #columns# from #sourceTableName# WITH(NOLOCK) 
			                        Where #dateTimeColumn# >= @FromTime and #dateTimeColumn# < @ToTime #RecordFilterResult#
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
            queryBuilder.Replace("#RecordFilterResult#", recordFilterResult);

            ExecuteNonQueryText(queryBuilder.ToString(), (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@FromTime", from));
                cmd.Parameters.Add(new SqlParameter("@ToTime", to));
                foreach (var prm in parameterValues)
                {
                    cmd.Parameters.Add(new SqlParameter(prm.Key, prm.Value));
                }
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

        public List<DataRecord> GetFilteredDataRecords(IDataRecordDataManagerGetFilteredDataRecordsContext context)
        {
            Columns = GetColumnNamesFromFieldNames(context.FieldNames);
            int parameterIndex = 0;
            Dictionary<string, Object> parameterValues = new Dictionary<string, object>();
            Data.SQL.RecordFilterSQLBuilder recordFilterSQLBuilder = new Data.SQL.RecordFilterSQLBuilder(GetColumnNameFromFieldName);
            String recordFilter = recordFilterSQLBuilder.BuildRecordFilter(context.FilterGroup, ref parameterIndex, parameterValues);
            string query = BuildQuery(context.FieldNames, context.LimitResult, context.Direction, recordFilter);

            return GetItemsText(query, DataRecordMapper, (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@FromTime", ToDBNullIfDefault(context.FromTime)));
                cmd.Parameters.Add(new SqlParameter("@ToTime", ToDBNullIfDefault(context.ToTime)));
                foreach (var prm in parameterValues)
                {
                    cmd.Parameters.Add(new SqlParameter(prm.Key, prm.Value));
                }
            });

        }

        public void GetDataRecords(DateTime? from, DateTime? to, RecordFilterGroup recordFilterGroup, Func<bool> shouldStop, Action<dynamic> onItemReady, string orderColumnName = null, bool isOrderAscending = false)
        {
            var recortTypeManager = new DataRecordTypeManager();
            var recordRuntimeType = recortTypeManager.GetDataRecordRuntimeType(_dataRecordStorage.DataRecordTypeId);
            if (recordRuntimeType == null)
                throw new NullReferenceException(String.Format("recordRuntimeType '{0}'", _dataRecordStorage.DataRecordTypeId));

            string dateTimeColumn = GetColumnNameFromFieldName(_dataRecordStorageSettings.DateTimeField);
            string tableName = GetTableNameWithSchema();

            StringBuilder sb_Query = new StringBuilder(string.Format(@"Select * From {0} WITH (NOLOCK)", tableName));

            List<string> queryFilters = new List<string>();
            if (from.HasValue)
                queryFilters.Add(string.Format("({0} >= @FromTime)", dateTimeColumn));

            if (to.HasValue)
                queryFilters.Add(string.Format("({0} < @ToTime)", dateTimeColumn));

            int parameterIndex = 0;
            Dictionary<string, Object> parameterValues = new Dictionary<string, object>();
            if (recordFilterGroup != null)
            {
                Data.SQL.RecordFilterSQLBuilder recordFilterSQLBuilder = new Data.SQL.RecordFilterSQLBuilder(GetColumnNameFromFieldName);
                string recordFilter = recordFilterSQLBuilder.BuildRecordFilter(recordFilterGroup, ref parameterIndex, parameterValues);

                if (!string.IsNullOrEmpty(recordFilter))
                    queryFilters.Add(recordFilter);
            }

            if (queryFilters.Count > 0)
                sb_Query.Append(string.Format(" Where {0} ", string.Join(" And ", queryFilters)));

            if (!string.IsNullOrEmpty(orderColumnName))
                sb_Query.Append(string.Format(" ORDER BY {0} {1} ", orderColumnName, isOrderAscending ? "" : "DESC"));

            sb_Query.Append(" OPTION (RECOMPILE) ");

            ExecuteReaderText(sb_Query.ToString(), (reader) =>
            {
                while (reader.Read())
                {
                    if (shouldStop != null && shouldStop())
                        break;

                    dynamic item = Activator.CreateInstance(recordRuntimeType) as dynamic;
                    this.DynamicManager.FillDataRecordFromReader(item, reader);
                    onItemReady(item);
                }
            }, (cmd) =>
            {
                if (from.HasValue)
                    cmd.Parameters.Add(new SqlParameter("@FromTime", from.Value));

                if (to.HasValue)
                    cmd.Parameters.Add(new SqlParameter("@ToTime", to.Value));

                foreach (var prm in parameterValues)
                    cmd.Parameters.Add(new SqlParameter(prm.Key, prm.Value));
            });
        }

        public List<DataRecord> GetAllDataRecords(List<string> columns)
        {
            Columns = GetColumnNamesFromFieldNames(columns);
            string query = BuildGetAllQuery();
            return GetItemsText(query, DataRecordMapper, (cmd) =>
            {
            });
        }

        public bool Insert(Dictionary<string, Object> fieldValues, int? createdUserId, int? modifiedUserId, out object insertedId)
        {
            Dictionary<string, Object> parameterValues = new Dictionary<string, Object>();
            bool withOutParameter = false;
            insertedId = null;
            SqlParameter sqlParameter = null;
            var effectedRows = ExecuteNonQueryText(BuildInsertQuery(fieldValues, parameterValues, createdUserId, modifiedUserId, ref withOutParameter), (cmd) =>
            {
                foreach (var prm in parameterValues)
                {
                    cmd.Parameters.Add(new SqlParameter(String.Format("{0}", prm.Key), prm.Value != null ? prm.Value : DBNull.Value));
                }
                if (withOutParameter)
                {
                    sqlParameter = new SqlParameter("@Id", SqlDbType.BigInt);
                    sqlParameter.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(sqlParameter);
                }

            });
            if (withOutParameter)
                insertedId = sqlParameter.Value;
            return effectedRows > 0;
        }

        public void InsertRecords(IEnumerable<dynamic> records)
        {
            var dbApplyStream = this.InitialiazeStreamForDBApply();
            foreach (var record in records)
            {
                this.WriteRecordToStream(record, dbApplyStream);
            }
            var readyStream = this.FinishDBApplyStream(dbApplyStream);
            this.ApplyStreamToDB(readyStream);
        }

        public bool Update(Dictionary<string, Object> fieldValues, int? modifiedUserId, RecordFilterGroup filterGroup)
        {
            Dictionary<string, Object> parameterValues = new Dictionary<string, Object>();
            return ExecuteNonQueryText(BuildUpdateQuery(fieldValues, parameterValues, modifiedUserId, filterGroup), (cmd) =>
            {
                foreach (var prm in parameterValues)
                {
                    cmd.Parameters.Add(new SqlParameter(String.Format("{0}", prm.Key), prm.Value != null ? prm.Value : DBNull.Value));
                }

            }) > 0;

        }

        public void UpdateRecords(IEnumerable<dynamic> records, List<string> fieldsToJoin, List<string> fieldsToUpdate)
        {
            List<string> columnsToJoin = this.GetColumnNamesFromFieldNames(fieldsToJoin);
            if (columnsToJoin == null || columnsToJoin.Count == 0)
                throw new NullReferenceException("columnsToJoin");

            List<string> columnsToUpdate = this.GetColumnNamesFromFieldNames(fieldsToUpdate);
            if (columnsToUpdate == null || columnsToUpdate.Count == 0)
                throw new NullReferenceException("columnsToUpdate");

            StringBuilder queryBuilder = new StringBuilder(@"UPDATE existingRecord
                                SET #COLUMNSUPDATE#
                                FROM #TABLE# existingRecord JOIN @UpdatedRecords updatedRecord ON #JOINCOLUMNS#");

            List<string> joinColumns = new List<string>();
            foreach (var joinColumn in columnsToJoin)
                joinColumns.Add(string.Format("existingRecord.{0} = updatedRecord.{0}", joinColumn));

            StringBuilder columnsUpdateBuilder = new StringBuilder();
            foreach (var columnToUpdate in columnsToUpdate)
            {
                if (columnsUpdateBuilder.Length > 0)
                    columnsUpdateBuilder.Append(", ");
                columnsUpdateBuilder.AppendLine(String.Format("{0} = updatedRecord.{0}", columnToUpdate));
            }

            queryBuilder.Replace("#TABLE#", GetTableNameWithSchema());
            queryBuilder.Replace("#JOINCOLUMNS#", string.Join(" And ", joinColumns));
            queryBuilder.Replace("#COLUMNSUPDATE#", columnsUpdateBuilder.ToString());

            DataTable dt = this.DynamicManager.ConvertDataRecordsToTable(records);
            ExecuteNonQueryText(queryBuilder.ToString(), (cmd) =>
            {
                SqlParameter prm = new SqlParameter("@UpdatedRecords", System.Data.SqlDbType.Structured);
                prm.TypeName = String.Format("{0}Type", GetTableNameWithSchema());
                prm.Value = dt;
                cmd.Parameters.Add(prm);
            });
        }

        public void DeleteRecords(DateTime from, DateTime to, RecordFilterGroup recordFilterGroup)
        {
            string dateTimeColumn = GetColumnNameFromFieldName(_dataRecordStorageSettings.DateTimeField);
            string tableName = GetTableNameWithSchema();
            Dictionary<string, Object> parameterValues = new Dictionary<string, object>();
            int parameterIndex = 0;

            string recordFilterResult = string.Empty;
            if (recordFilterGroup != null)
            {
                Data.SQL.RecordFilterSQLBuilder recordFilterSQLBuilder = new Data.SQL.RecordFilterSQLBuilder(GetColumnNameFromFieldName);
                string recordFilter = recordFilterSQLBuilder.BuildRecordFilter(recordFilterGroup, ref parameterIndex, parameterValues);
                recordFilterResult = !string.IsNullOrEmpty(recordFilter) ? string.Format(" AND {0} ", recordFilter) : string.Empty;
            }

            string query = string.Format(@"DELETE {0} from {0} WITH (NOLOCK)
                                           WHERE ({1} >= @FromTime) 
                                           AND ({1} < @ToTime) {2}", tableName, dateTimeColumn, recordFilterResult);

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

        public void DeleteRecords(DateTime dateTime, RecordFilterGroup recordFilterGroup)
        {
            string dateTimeColumn = GetColumnNameFromFieldName(_dataRecordStorageSettings.DateTimeField);
            string tableName = GetTableNameWithSchema();
            Dictionary<string, Object> parameterValues = new Dictionary<string, object>();
            int parameterIndex = 0;

            string recordFilterResult = string.Empty;
            if (recordFilterGroup != null)
            {
                Data.SQL.RecordFilterSQLBuilder recordFilterSQLBuilder = new Data.SQL.RecordFilterSQLBuilder(GetColumnNameFromFieldName);
                string recordFilter = recordFilterSQLBuilder.BuildRecordFilter(recordFilterGroup, ref parameterIndex, parameterValues);
                recordFilterResult = !string.IsNullOrEmpty(recordFilter) ? string.Format(" AND {0} ", recordFilter) : string.Empty;
            }

            string query = string.Format(@"DELETE {0} from {0} WITH (NOLOCK)
                                           WHERE ({1} = @dateTime) {2}", tableName, dateTimeColumn, recordFilterResult);


            ExecuteNonQueryText(query, (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@dateTime", dateTime));
                foreach (var prm in parameterValues)
                {
                    cmd.Parameters.Add(new SqlParameter(prm.Key, prm.Value));
                }
            });
        }

        public bool AreDataRecordsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated(GetTableNameWithSchema(), ref updateHandle);
        }

        public int GetDBQueryMaxParameterNumber()
        {
            return base.GetSQLQueryMaxParameterNumber();
        }

        public DateTime? GetMinDateTimeAfterId(long id, string idFieldName, string dateTimeFieldName)
        {
            DateTime? minDate = null;
            string tableName = GetTableNameWithSchema();
            string query = string.Format(@"Select Min({0}) as MinDate from {1} WITH (NOLOCK)
                                           WHERE {2}  > {3}", dateTimeFieldName, tableName, idFieldName, id);

            ExecuteReaderText(query, (reader) =>
            {
                while (reader.Read())
                {
                    minDate = GetReaderValue<DateTime?>(reader, "MinDate");
                }
            }, null);

            return minDate;
        }

        #region Private Methods

        private string GetTableNameWithSchema()
        {
            string tableName = this._dataRecordStorageSettings.TableName;
            if (!String.IsNullOrEmpty(this._dataRecordStorageSettings.TableSchema))
                tableName = String.Format("{0}.{1}", this._dataRecordStorageSettings.TableSchema, this._dataRecordStorageSettings.TableName);
            return tableName;
        }

        private string GetRecordStorageCreateTableQuery(string tableName, bool isTempTable)
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

        private string BuildDropAndCreateTableTypeScript()
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

        private string GetRecordStorageAlterTableQuery(SQLDataRecordStorageSettings existingDataRecordSettings)
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

        private void SetAddAndAlterColumnDefinitions(SQLDataRecordStorageSettings existingDataRecordSettings, out List<string> addColumnDefinitions, out List<string> alterColumnDefinitions)
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

        private void AppendErrorCheck(StringBuilder sql)
        {
            sql.Append(" IF @@ERROR <> 0 BEGIN ROLLBACK TRAN END;");
        }

        private string BuildInsertQuery(Dictionary<string, Object> fieldValues, Dictionary<string, Object> parameterValues, int? createdUserId, int? modifiedUserId, ref bool withOutParameter)
        {
            StringBuilder queryBuilder = new StringBuilder();

            if (fieldValues == null || fieldValues.Count == 0)
                throw new Exception("fieldValues should not be null or empty.");

            int parameterIndex = 0;
            string tableName = GetTableNameWithSchema();

            StringBuilder ifNotExistsQueryBuilder = new StringBuilder();
            StringBuilder columnNamesBuilder = new StringBuilder();
            StringBuilder valuesQuery = new StringBuilder();
            foreach (var fieldValue in fieldValues)
            {
                var sqlDataRecordStorageColumn = GetColumnFromFieldName(fieldValue.Key);

                var parameter = GenerateParameterName(ref parameterIndex);
                DataRecordField matchingField = DataRecordType.Fields.FindRecord(itm => itm.Name == fieldValue.Key);
                if (matchingField.Type.StoreValueSerialized)
                {
                    parameterValues.Add(parameter, matchingField.Type.SerializeValue(new SerializeDataRecordFieldValueContext
                    {
                        Object = fieldValue.Value
                    }));
                }
                else
                    parameterValues.Add(parameter, fieldValue.Value);

                if (sqlDataRecordStorageColumn.IsUnique)
                {
                    if (ifNotExistsQueryBuilder.Length > 0)
                    {
                        ifNotExistsQueryBuilder.Append(" AND ");
                    }
                    ifNotExistsQueryBuilder.AppendFormat("{0} = {1}", sqlDataRecordStorageColumn.ColumnName, parameter);
                }
                if (parameterIndex != 1)
                {
                    columnNamesBuilder.Append(",");
                    valuesQuery.Append(",");
                }
                columnNamesBuilder.AppendFormat(@" {0} ", sqlDataRecordStorageColumn.ColumnName);
                valuesQuery.AppendFormat(@" {0} ", parameter);
            }
            var idColumn = GetIdColumn();
            if (idColumn != null)
            {
                withOutParameter = true;
            }

            if (createdUserId.HasValue && _dataRecordStorage.Settings.CreatedByField != null)
            {
                var createdByColumn = GetColumnFromFieldName(_dataRecordStorage.Settings.CreatedByField);
                if (createdByColumn != null)
                {
                    var parameter = GenerateParameterName(ref parameterIndex);
                    parameterValues.Add(parameter, createdUserId.Value);

                    if (parameterIndex != 1)
                    {
                        columnNamesBuilder.Append(",");
                        valuesQuery.Append(",");
                    }

                    columnNamesBuilder.AppendFormat(@" {0} ", createdByColumn.ColumnName);
                    valuesQuery.AppendFormat(@" {0} ", parameter);
                }
            }

            if (_dataRecordStorage.Settings.CreatedTimeField != null)
            {
                var createdTimeColumn = GetColumnFromFieldName(_dataRecordStorage.Settings.CreatedTimeField);
                if (createdTimeColumn != null)
                {
                    if (parameterIndex != 1)
                    {
                        columnNamesBuilder.Append(",");
                        valuesQuery.Append(",");
                    }

                    columnNamesBuilder.AppendFormat(@" {0} ", createdTimeColumn.ColumnName);
                    valuesQuery.AppendFormat(@"  getdate()  ");
                }

            }

            if (modifiedUserId.HasValue && _dataRecordStorage.Settings.LastModifiedByField != null)
            {
                var modifiedByColumn = GetColumnFromFieldName(_dataRecordStorage.Settings.LastModifiedByField);
                if (modifiedByColumn != null)
                {
                    var parameter = GenerateParameterName(ref parameterIndex);
                    parameterValues.Add(parameter, modifiedUserId.Value);

                    if (parameterIndex != 1)
                    {
                        columnNamesBuilder.Append(",");
                        valuesQuery.Append(",");
                    }

                    columnNamesBuilder.AppendFormat(@" {0} ", modifiedByColumn.ColumnName);
                    valuesQuery.AppendFormat(@" {0} ", parameter);
                }
            }

            if (_dataRecordStorage.Settings.LastModifiedTimeField != null)
            {
                var modifiedTimeColumn = GetColumnFromFieldName(_dataRecordStorage.Settings.LastModifiedTimeField);
                if (modifiedTimeColumn != null)
                {
                    if (parameterIndex != 1)
                    {
                        columnNamesBuilder.Append(",");
                        valuesQuery.Append(",");
                    }

                    columnNamesBuilder.AppendFormat(@" {0} ", modifiedTimeColumn.ColumnName);
                    valuesQuery.AppendFormat(@" getdate() ");
                }

            }



            if (ifNotExistsQueryBuilder.Length > 0)
            {
                queryBuilder.Append(@" IF NOT EXISTS(SELECT 1 FROM #TABLENAME# WHERE #IFNOTEXISTSQUERY#) ");
                queryBuilder.Replace("#IFNOTEXISTSQUERY#", ifNotExistsQueryBuilder.ToString());
            }
            queryBuilder.Append(@"BEGIN  INSERT INTO #TABLENAME# (#INSERTCOLUMNNAMES#) VALUES (#INSERTPARAMETERS#) #SETIDENTITY#  END");
            queryBuilder.Replace("#SETIDENTITY#", idColumn != null ? " Set @Id = SCOPE_IDENTITY() " : " ");
            queryBuilder.Replace("#INSERTCOLUMNNAMES#", columnNamesBuilder.ToString());
            queryBuilder.Replace("#INSERTPARAMETERS#", valuesQuery.ToString());
            queryBuilder.Replace("#TABLENAME#", tableName);

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
                    DataRecordField matchingField = DataRecordFieldsByName.GetRecord(column.ValueExpression);
                    matchingField.ThrowIfNull("matchingField", column.ValueExpression);
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
                    else
                    {
                        value = matchingField.Type.ParseValueToFieldType(new DataRecordFieldTypeParseValueToFieldTypeContext(value));
                    }

                    fieldValues.Add(column.ValueExpression, value);
                }
            }
            return new DataRecord { RecordTime = recordTime, FieldValues = fieldValues };
        }

        private string GetColumnNameFromFieldName(string fieldName)
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

        private SQLDataRecordStorageColumn GetColumnFromFieldName(string fieldName)
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

        private DataRecordField GetIdColumn()
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

        private List<string> GetColumnNamesFromFieldNames(List<string> fieldNames)
        {
            if (fieldNames == null || fieldNames.Count == 0)
                return null;
            List<string> result = new List<string>();
            foreach (string fieldName in fieldNames)
            {
                string columnName = GetColumnNameFromFieldName(fieldName);
                if (string.Compare(columnName, "null", true) != 0)
                    result.Add(columnName);
            }
            return result;
        }

        private string BuildQuery(List<string> fieldNames, int? limitResult, OrderDirection orderDirection, string recordFilter)
        {
            string dateTimeColumn = GetColumnNameFromFieldName(_dataRecordStorageSettings.DateTimeField);
            string tableName = GetTableNameWithSchema();

            string recordFilterResult = !string.IsNullOrEmpty(recordFilter) ? string.Format(" and {0} ", recordFilter) : string.Empty;
            string orderResult = string.Format(" Order By {0} {1} ", dateTimeColumn, orderDirection == OrderDirection.Ascending ? "ASC" : "DESC");
            StringBuilder str = new StringBuilder(string.Format(@"  select {0} {1} from {2} WITH (NOLOCK)
                                                                    where (@FromTime is null or {3} >= @FromTime) 
                                                                    and (@ToTime is null or {3} <= @ToTime) {4} {5} OPTION (RECOMPILE)",
                                                                    limitResult.HasValue ? string.Format(" Top {0} ", limitResult.Value) : "", string.Join<string>(",", GetColumnNamesFromFieldNames(fieldNames)),
                                                                    tableName, dateTimeColumn, recordFilterResult, orderResult));
            //input.SortByColumnName = dateTimeColumn;
            return str.ToString();
        }

        private string BuildGetAllQuery()
        {
            string tableName = GetTableNameWithSchema();
            StringBuilder str = new StringBuilder(string.Format(@"  select {0} from {1} WITH (NOLOCK) ",
                                                                     string.Join<string>(",", Columns),
                                                                    tableName));
            return str.ToString();
        }

        private string BuildUpdateQuery(Dictionary<string, Object> fieldValues, Dictionary<string, Object> parameterValues, int? modifiedUserId, RecordFilterGroup filterGroup)
        {
            if (fieldValues == null || fieldValues.Count == 0)
                throw new Exception("fieldValues should not be null or empty.");
            StringBuilder queryBuilder = new StringBuilder();

            int parameterIndex = 0;
            string tableName = GetTableNameWithSchema();
            StringBuilder whereQuery = new StringBuilder();
            StringBuilder valuesQuery = new StringBuilder();
            StringBuilder ifNotExistsQueryBuilder = new StringBuilder();
            var idColumn = GetIdColumn();
            var shouldAddIfExist = false;

            foreach (var fieldValue in fieldValues)
            {
                var sqlDataRecordStorageColumn = GetColumnFromFieldName(fieldValue.Key);
                var parameter = GenerateParameterName(ref parameterIndex);

                DataRecordField matchingField = DataRecordType.Fields.FindRecord(itm => itm.Name == fieldValue.Key);
                if (matchingField.Type.StoreValueSerialized)
                {
                    parameterValues.Add(parameter, matchingField.Type.SerializeValue(new SerializeDataRecordFieldValueContext
                    {
                        Object = fieldValue.Value
                    }));
                }
                else
                    parameterValues.Add(parameter, fieldValue.Value);

                if (sqlDataRecordStorageColumn.IsUnique)
                {
                    if (ifNotExistsQueryBuilder.Length > 0)
                    {
                        ifNotExistsQueryBuilder.Append(" AND ");
                    }
                    if (idColumn.Name == fieldValue.Key)
                    {
                        ifNotExistsQueryBuilder.AppendFormat("{0} != {1}", sqlDataRecordStorageColumn.ColumnName, parameter);
                        if (whereQuery.Length != 0)
                            whereQuery.Append(" AND ");
                        whereQuery.AppendFormat(@" {0} = {1}  ", sqlDataRecordStorageColumn.ColumnName, parameter);
                    }
                    else
                    {
                        shouldAddIfExist = true;
                        ifNotExistsQueryBuilder.AppendFormat("{0} = {1}", sqlDataRecordStorageColumn.ColumnName, parameter);
                    }


                }
                if (idColumn.Name != fieldValue.Key)
                {
                    if (valuesQuery.Length != 0)
                        valuesQuery.Append(",");
                    valuesQuery.AppendFormat(@" {0} = {1}  ", sqlDataRecordStorageColumn.ColumnName, parameter);
                }
            }

            if (modifiedUserId.HasValue && _dataRecordStorage.Settings.LastModifiedByField != null)
            {
                var modifiedByColumn = GetColumnFromFieldName(_dataRecordStorage.Settings.LastModifiedByField);
                if (modifiedByColumn != null)
                {
                    var parameter = GenerateParameterName(ref parameterIndex);
                    parameterValues.Add(parameter, modifiedUserId.Value);

                    if (valuesQuery.Length != 0)
                        valuesQuery.Append(",");
                    valuesQuery.AppendFormat(@" {0} = {1}  ", modifiedByColumn.ColumnName, parameter);
                }

            }

            if (_dataRecordStorage.Settings.LastModifiedTimeField != null)
            {
                var modifiedTimeColumn = GetColumnFromFieldName(_dataRecordStorage.Settings.LastModifiedTimeField);
                if (modifiedTimeColumn != null)
                {
                    if (valuesQuery.Length != 0)
                        valuesQuery.Append(",");
                    valuesQuery.AppendFormat(@" {0} = getdate()  ", modifiedTimeColumn.ColumnName);
                }

            }

            if (shouldAddIfExist)
            {
                queryBuilder.Append(@" IF NOT EXISTS(SELECT 1 FROM #TABLENAME# WHERE #IFNOTEXISTSQUERY#) ");
                queryBuilder.Replace("#IFNOTEXISTSQUERY#", ifNotExistsQueryBuilder.ToString());
            }
            if (filterGroup != null)
            {
                string recordFilterResult = string.Empty;
                if (whereQuery.Length > 0)
                {
                    whereQuery.Append(" AND ");
                }
                Data.SQL.RecordFilterSQLBuilder recordFilterSQLBuilder = new Data.SQL.RecordFilterSQLBuilder(GetColumnNameFromFieldName);
                string recordFilter = recordFilterSQLBuilder.BuildRecordFilter(filterGroup, ref parameterIndex, parameterValues);
                recordFilterResult = !string.IsNullOrEmpty(recordFilter) ? string.Format(" {0} ", recordFilter) : string.Empty;
                whereQuery.Append(recordFilterResult);
            }

            queryBuilder.Append(@"BEGIN  UPDATE #TABLENAME# SET #VALUESQUERY# WHERE #WHEREQUERY#  END");
            queryBuilder.Replace("#VALUESQUERY#", valuesQuery.ToString());
            queryBuilder.Replace("#WHEREQUERY#", whereQuery.ToString());
            queryBuilder.Replace("#TABLENAME#", tableName);
            return queryBuilder.ToString();
        }

        private string GenerateParameterName(ref int parameterIndex)
        {
            return String.Format("@Prm_{0}", parameterIndex++);
        }

        #endregion
    }
}