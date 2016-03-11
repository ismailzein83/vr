using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.SQLDataStorage
{
    internal class SQLRecordStorageDataManager : BaseSQLDataManager, IDataRecordDataManager, ISummaryRecordDataManager
    {
        SQLDataStoreSettings _dataStoreSettings;
        SQLDataRecordStorageSettings _dataRecordStorageSettings;
        DataRecordStorage _dataRecordStorage;
        SummaryTransformationDefinition _summaryTransformationDefinition;

        internal SQLRecordStorageDataManager(SQLDataStoreSettings dataStoreSettings, SQLDataRecordStorageSettings dataRecordStorageSettings)
            : base(dataStoreSettings.ConnectionString, false)
        {
            this._dataStoreSettings = dataStoreSettings;
            this._dataRecordStorageSettings = dataRecordStorageSettings;
        }

        internal SQLRecordStorageDataManager(SQLDataStoreSettings dataStoreSettings, SQLDataRecordStorageSettings dataRecordStorageSettings, DataRecordStorage dataRecordStorage)
            : this(dataStoreSettings, dataRecordStorageSettings)
        {
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

        IBulkInsertWriter _bulkInsertWriter;

        public void WriteRecordToStream(object record, object dbApplyStream)
        {
            BuildBulkInsertWriter();
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            _bulkInsertWriter.WriteRecordToStream(record, streamForBulkInsert);
        }

        private void BuildBulkInsertWriter()
        {
            if (_bulkInsertWriter == null)
            {
                if (_dataRecordStorage == null)
                    throw new NullReferenceException("_dataRecordStorage");
                DynamicTypeGenerator dynamicTypeGenerator = new DynamicTypeGenerator();
                _bulkInsertWriter = dynamicTypeGenerator.GetBulkInsertWriter(_dataRecordStorage.DataRecordStorageId, _dataRecordStorageSettings);
            }
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            string tableName = this._dataRecordStorageSettings.TableName;
            if (!String.IsNullOrEmpty(this._dataRecordStorageSettings.TableSchema))
                tableName = String.Format("{0}.{1}", this._dataRecordStorageSettings.TableSchema, this._dataRecordStorageSettings.TableName);
            BuildBulkInsertWriter();
            return new StreamBulkInsertInfo
            {
                TableName = tableName,
                Stream = streamForBulkInsert,
                ColumnNames = _bulkInsertWriter.ColumnNames,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
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
            query.Append(String.Format("CREATE TABLE {0}", tableName));
            
            List<string> columnDefinitions = new List<string>();
            
            foreach (var column in _dataRecordStorageSettings.Columns)
            {
                columnDefinitions.Add(String.Format("{0} {1}", column.ColumnName, column.SQLDataType));
            }

            query.Append(String.Format(" ({0});", string.Join(",", columnDefinitions)));
            
            return query.ToString();
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

        public void InsertSummaryRecords(List<dynamic> records)
        {
            var dbApplyStream = this.InitialiazeStreamForDBApply();
            foreach(var record in records)
            {
                this.WriteRecordToStream(record, dbApplyStream);
            }
            var readyStream = this.FinishDBApplyStream(dbApplyStream);
            this.ApplyStreamToDB(readyStream);
        }

        public void UpdateSummaryRecords(List<dynamic> records)
        {
            throw new NotImplementedException();
        }

        public List<dynamic> GetExistingSummaryRecords(DateTime batchStart, DateTime batchEnd)
        {
            var recortTypeManager = new DataRecordTypeManager();
            var recordRuntimeType = recortTypeManager.GetDataRecordRuntimeType(_dataRecordStorage.DataRecordTypeId);
            if (recordRuntimeType == null)
                throw new NullReferenceException(String.Format("recordRuntimeType '{0}'", _dataRecordStorage.DataRecordTypeId));
            StringBuilder queryBuilder = new StringBuilder(@"SELECT #COLUMNS# FROM #TABLE# WHERE #BATCHSTARTCOLUMN# >= @BatchStart AND #BATCHENDCOLUMN# < @BatchEnd");
            StringBuilder columnsBuilder = new StringBuilder();
            string batchStartColumnName = null;
            string batchEndColumnName = null;
            foreach(var columnSetting in _dataRecordStorageSettings.Columns)
            {
                if (columnsBuilder.Length > 0)
                    columnsBuilder.Append(", ");
                columnsBuilder.Append(columnSetting.ColumnName);
                if (columnSetting.ValueExpression == _summaryTransformationDefinition.BatchStartFieldName)
                    batchStartColumnName = columnSetting.ColumnName;
                else if (columnSetting.ValueExpression == _summaryTransformationDefinition.BatchEndFieldName)
                    batchEndColumnName = columnSetting.ColumnName;
            }
            if (batchStartColumnName == null)
                throw new NullReferenceException(String.Format("batchStartColumnName '{0}'", _summaryTransformationDefinition.BatchStartFieldName));
            if (batchEndColumnName == null)
                throw new NullReferenceException(String.Format("batchEndColumnName '{0}'", _summaryTransformationDefinition.BatchEndFieldName));

            queryBuilder.Replace("#COLUMNS#", columnsBuilder.ToString());
            queryBuilder.Replace("#TABLE#", _dataRecordStorageSettings.TableName);

            queryBuilder.Replace("#BATCHSTARTCOLUMN#", batchStartColumnName);
            queryBuilder.Replace("#BATCHENDCOLUMN#", batchEndColumnName);
            return GetItemsText(queryBuilder.ToString(),
                (reader) =>
                {
                    dynamic item = Activator.CreateInstance(recordRuntimeType) as dynamic;
                    foreach (var columnSetting in _dataRecordStorageSettings.Columns)
                    {
                        item[columnSetting.ValueExpression] = reader[columnSetting.ColumnName];
                    }
                    return item;
                },
                (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@BatchStart", batchStart));
                    cmd.Parameters.Add(new SqlParameter("@BatchEnd", batchEnd));
                });
        }
    }
}
