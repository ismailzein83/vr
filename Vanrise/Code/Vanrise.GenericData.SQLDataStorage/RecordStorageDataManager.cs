using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.SQLDataStorage
{
    internal class RecordStorageDataManager : BaseSQLDataManager, IDataRecordDataManager
    {
        DataStore _dataStore;
        DataRecordStorage _dataRecordStorage;

        internal RecordStorageDataManager(DataStore dataStore, DataRecordStorage dataRecordStorage)
        {
            this._dataStore = dataStore;
            this._dataRecordStorage = dataRecordStorage;
        }

        protected override string GetConnectionString()
        {
            var dataStoreSettings = _dataStore.Settings as SQLDataStoreSettings;
            if (dataStoreSettings == null)
                throw new ArgumentNullException("dataStoreSettings");
            return dataStoreSettings.ConnectionString;
        }

        public void ApplyStreamToDB(object stream)
        {
            base.InsertBulkToTable(stream as BaseBulkInsertInfo);
        }

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(object record, object dbApplyStream)
        {
            throw new NotImplementedException();
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            var dataRecordStorageSettings = this._dataRecordStorage.Settings as SQLDataRecordStorageSettings;
            if (dataRecordStorageSettings == null)
                throw new ArgumentNullException("dataRecordStorageSettings");
            return new StreamBulkInsertInfo
            {
                TableName = dataRecordStorageSettings.TableName,
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
            };
        }

        public bool CreateSQLRecordStorageTable(SQLDataRecordStorageSettings settings)
        {
            string query = GetRecordStorageCreateTableQuery(settings);
            int affectedRows = ExecuteNonQueryText(query, null);
            return affectedRows == 1;
        }

        #region Private Methods

        string GetRecordStorageCreateTableQuery(SQLDataRecordStorageSettings settings)
        {
            if (settings == null || settings.TableName == null || settings.Columns == null)
                throw new ArgumentNullException("settings.Property");

            StringBuilder builder = new StringBuilder();

            builder.Append(String.Format("CREATE TABLE [genericdata].[{0}] (", settings.TableName));
            builder.Append(String.Format(" [ID] [int] IDENTITY(1,1) NOT NULL,"));

            foreach (SQLDataRecordStorageColumn column in settings.Columns)
            {
                if (column.ColumnName == null || column.SQLDataType == null)
                    throw new ArgumentNullException("column.Property");
                builder.Append(String.Format(" [{0}] [{1}],", column.ColumnName, column.SQLDataType));
            }

            builder.Append(String.Format(" CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED ([ID] ASC)", settings.TableName));
            builder.Append(String.Format(" WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY]"));

            return builder.ToString();
        }

        #endregion
    }
}
