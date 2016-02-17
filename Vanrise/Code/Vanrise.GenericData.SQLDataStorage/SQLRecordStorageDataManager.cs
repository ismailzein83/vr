using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.SQLDataStorage
{
    internal class SQLRecordStorageDataManager : BaseSQLDataManager, IDataRecordDataManager
    {
        SQLDataStoreSettings _dataStoreSettings;
        SQLDataRecordStorageSettings _dataRecordStorageSettings;
        int _dataRecordStorageId;
        internal SQLRecordStorageDataManager(SQLDataStoreSettings dataStoreSettings, SQLDataRecordStorageSettings dataRecordStorageSettings)
            : base(dataStoreSettings.ConnectionString, false)
        {
            this._dataStoreSettings = dataStoreSettings;
            this._dataRecordStorageSettings = dataRecordStorageSettings;
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
                DynamicTypeGenerator dynamicTypeGenerator = new DynamicTypeGenerator();
                _bulkInsertWriter = dynamicTypeGenerator.GetBulkInsertWriter(_dataRecordStorageId, _dataRecordStorageSettings);
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
    }
}
