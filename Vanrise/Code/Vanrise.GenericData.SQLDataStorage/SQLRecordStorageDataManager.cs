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

        public void WriteRecordToStream(object record, object dbApplyStream)
        {
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

        public bool CreateSQLRecordStorageTable()
        {
            string query = GetRecordStorageCreateTableQuery();
            int affectedRows = ExecuteNonQueryText(query, null);
            return affectedRows == 1;
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
            builder.Append(String.Format(" [ID] [int] IDENTITY(1,1) NOT NULL,"));

            foreach (SQLDataRecordStorageColumn column in _dataRecordStorageSettings.Columns)
            {
                if (column.ColumnName == null || column.SQLDataType == null)
                    throw new ArgumentNullException("column.Property");
                builder.Append(String.Format(" [{0}] [{1}],", column.ColumnName, column.SQLDataType));
            }

            builder.Append(String.Format(" CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED ([ID] ASC)", _dataRecordStorageSettings.TableName));
            builder.Append(String.Format(" WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY]"));

            return builder.ToString();
        }

        #endregion
    }
}
