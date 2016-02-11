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
            if(_bulkInsertWriter == null)
            {
                DynamicTypeGenerator dynamicTypeGenerator = new DynamicTypeGenerator();
                _bulkInsertWriter = dynamicTypeGenerator.GetBulkInsertWriter(_dataRecordStorageId, _dataRecordStorageSettings);
            }
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            
            throw new NotImplementedException();
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = this._dataRecordStorageSettings.TableName,
                Stream = streamForBulkInsert,
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
            ExecuteNonQueryText(GetRecordStorageAlterTableQuery(existingDataRecordSettings), null);
        }

        #region Private Methods

        string GetRecordStorageCreateTableQuery()
        {
            if (_dataRecordStorageSettings.TableName == null)
                throw new ArgumentNullException("_dataRecordStorageSettings.TableName");

            if (_dataRecordStorageSettings.Columns == null)
                throw new ArgumentNullException("_dataRecordStorageSettings.Columns");

            StringBuilder builder = new StringBuilder();
            builder.Append(String.Format("CREATE TABLE [genericdata].[{0}] (", _dataRecordStorageSettings.TableName));

            int columnCount = _dataRecordStorageSettings.Columns.Count;
            for (int i = 0; i < columnCount; i++)
            {
                var column = _dataRecordStorageSettings.Columns[i];
                builder.Append(String.Format("[{0}] [{1}]{2}", column.ColumnName, column.SQLDataType, (i < columnCount - 1) ? "," : null));
            }

            builder.Append(")");
            return builder.ToString();
        }

        string GetRecordStorageAlterTableQuery(SQLDataRecordStorageSettings existingDataRecordSettings)
        {
            if (_dataRecordStorageSettings.TableName == null)
                throw new ArgumentNullException("_dataRecordStorageSettings.TableName");

            if (_dataRecordStorageSettings.Columns == null)
                throw new ArgumentNullException("_dataRecordStorageSettings.Columns");

            StringBuilder builder = new StringBuilder();
            builder.Append(String.Format("sp_rename 'genericdata.{0}', '{1}';", existingDataRecordSettings.TableName, _dataRecordStorageSettings.TableName));
            builder.Append(String.Format("ALTER TABLE [genericdata].[{0}] ADD", _dataRecordStorageSettings.TableName));

            var columnsToAdd = GetColumnsToAdd(existingDataRecordSettings);
            var columnCount = columnsToAdd.Count();
            for (int i = 0; i < columnCount; i++)
            {
                var column = columnsToAdd.ElementAt(i);
                builder.Append(String.Format(" {0} {1}{2}", column.ColumnName, column.SQLDataType, (i < columnCount - 1) ? "," : ";"));
            }

            return builder.ToString();
        }

        IEnumerable<SQLDataRecordStorageColumn> GetColumnsToAdd(SQLDataRecordStorageSettings existingDataRecordSettings)
        {
            List<SQLDataRecordStorageColumn> columnsToAdd = new List<SQLDataRecordStorageColumn>();
            foreach (var column in _dataRecordStorageSettings.Columns)
            {
                var matchExistingColumnState = existingDataRecordSettings.Columns.FirstOrDefault(itm => itm.ColumnName == column.ColumnName);
                if (matchExistingColumnState == null)
                    columnsToAdd.Add(column);
                else if (column.SQLDataType != matchExistingColumnState.SQLDataType)
                    throw new Exception(String.Format("Cannot change data type of column '{0}'", column.ColumnName));
            }
            return columnsToAdd;
        }

        #endregion
    }
}
