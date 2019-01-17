using System;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Data.RDB;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class StateBackupDataManager : IStateBackupDataManager
    {

        #region Private Memebers

        private StateBackupTypeBehavior _stateBackupBehavior = null;

        #endregion
        #region RDB

        static string TABLE_NAME = "TOneWhS_BE_StateBackup";
        const string COL_ID = "ID";
        const string COL_ConfigID = "ConfigID";
        const string COL_Info = "Info";
        const string COL_BackupDate = "BackupDate";
        const string COL_RestoreDate = "RestoreDate";
        const string COL_BackupByUserID = "BackupByUserID";
        const string COL_RestoredByUserID = "RestoredByUserID";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";

        static StateBackupDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_ConfigID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Info, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_BackupDate, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_RestoreDate, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_BackupByUserID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_RestoredByUserID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "StateBackup",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime

            });
        }
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("TOneWhS_BE", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }

        #endregion

        #region IStateBackupDataManager Members

        public object BackupData(StateBackupType backupType)
        {
            this.PrepareData(backupType);

            long stateBackupId;
            bool inserted = Insert(backupType, out stateBackupId);
            if (inserted)
            {

            }
            return stateBackupId;
        }

        public bool RestoreData(long stateBackupId, StateBackupType stateBackupType, int userId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Entities.StateBackup> GetFilteredStateBackups(StateBackupQuery input)
        {
            throw new NotImplementedException();
        }

        public Entities.StateBackup GetStateBackup(long stateBackupId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Entities.StateBackup> GetStateBackupsAfterId(long stateBackupId)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Methods

        private bool Insert(StateBackupType backupType, out long stateBackupId)
        {
            stateBackupId = 0;
            var queryContext = new RDBQueryContext(GetDataProvider());

            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.AddSelectGeneratedId();

            insertQuery.Column(COL_ConfigID).Value(backupType.ConfigId);
            insertQuery.Column(COL_BackupDate).DateNow();
            insertQuery.Column(COL_BackupByUserID).Value(backupType.UserId);
            insertQuery.Column(COL_Info).Value(Serializer.Serialize(backupType));

            var returnedValue = queryContext.ExecuteScalar().NullableLongValue;
            if (returnedValue.HasValue)
            {
                stateBackupId = returnedValue.Value;
                return true;
            }
            stateBackupId = 0;
            return false;
        }
        private void PrepareData(StateBackupType backupType)
        {
            if (backupType is StateBackupAllSaleEntities)
                _stateBackupBehavior = new StateBackupAllSaleEntitiesBehavior();
            else if (backupType is StateBackupSaleEntity)
                _stateBackupBehavior = new StateBackupSaleEntityBehavior();
            else if (backupType is StateBackupSupplier)
                _stateBackupBehavior = new StateBackupSupplierBehavior();

            if (_stateBackupBehavior == null)
                throw new InvalidOperationException("Backup Type is not specified");

            _stateBackupBehavior.Data = backupType;
            string backUpDataBaseName = System.Configuration.ConfigurationManager.AppSettings["StateBackupDatabase"];

            if (string.IsNullOrEmpty(backUpDataBaseName))
                throw new ArgumentNullException("StateBackupDatabase key is missing");

            _stateBackupBehavior.BackupDatabaseName = backUpDataBaseName;
        }

        #endregion

    }
}
