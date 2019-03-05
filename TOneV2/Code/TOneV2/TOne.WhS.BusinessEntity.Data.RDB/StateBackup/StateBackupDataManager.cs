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

        static string TABLE_ALIAS = "sb";
        static string TABLE_NAME = "TOneWhS_BE_StateBackup";
        const string COL_ID = "ID";
        const string COL_ConfigID = "ConfigID";
        const string COL_Info = "Info";
        const string COL_BackupDate = "BackupDate";
        const string COL_RestoreDate = "RestoreDate";
        const string COL_BackupByUserID = "BackupByUserID";
        const string COL_RestoredByUserID = "RestoredByUserID";

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

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "StateBackup",
                Columns = columns,
                IdColumnName = COL_ID

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

            bool inserted = Insert(backupType, out var stateBackupId);
            if (inserted)
            {
                var queryContext = new RDBQueryContext(GetDataProvider());
                _stateBackupBehavior.GetBackupQueryContext(queryContext, stateBackupId);
                queryContext.ExecuteNonQuery(true);
            }
            return stateBackupId;
        }

        public bool RestoreData(long stateBackupId, StateBackupType stateBackupType, int userId)
        {
            this.PrepareData(stateBackupType);
            var queryContext = new RDBQueryContext(GetDataProvider());
            _stateBackupBehavior.GetRestoreCommands(queryContext, stateBackupId);
            SetUpdateQuery(queryContext, stateBackupId, stateBackupType.UserId);
            return queryContext.ExecuteNonQuery(true) > 0;
        }

        public IEnumerable<Entities.StateBackup> GetFilteredStateBackups(StateBackupQuery input)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            if (input.BackupTypeFilterConfigId.HasValue)
                whereContext.EqualsCondition(COL_ConfigID).Value(input.BackupTypeFilterConfigId.Value);
            if (input.From.HasValue)
                whereContext.GreaterOrEqualCondition(COL_BackupDate).Value(input.From.Value);
            if (input.To.HasValue)
                whereContext.LessOrEqualCondition(COL_BackupDate).Value(input.To.Value);

            return queryContext.GetItems(StateBackupMapper);
        }

        public Entities.StateBackup GetStateBackup(long stateBackupId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_ID).Value(stateBackupId);

            return queryContext.GetItem(StateBackupMapper);
        }

        public IEnumerable<Entities.StateBackup> GetStateBackupsAfterId(long stateBackupId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.GreaterThanCondition(COL_ID).Value(stateBackupId);

            return queryContext.GetItems(StateBackupMapper);
        }

        #endregion

        #region Private Methods

        private void SetUpdateQuery(RDBQueryContext queryContext, long stateBackupId, int userId)
        {
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            updateQuery.Column(COL_RestoreDate).DateNow();
            updateQuery.Column(COL_RestoredByUserID).Value(userId);

            updateQuery.Where().EqualsCondition(COL_ID).Value(stateBackupId);
        }
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

        #region Private Mappers

        private Entities.StateBackup StateBackupMapper(IRDBDataReader reader)
        {
            return new Entities.StateBackup
            {
                StateBackupId = reader.GetLong(COL_ID),
                BackupTypeConfigId = reader.GetGuidWithNullHandling(COL_ConfigID),
                Info = Serializer.Deserialize<StateBackupType>(reader.GetString(COL_Info)),
                BackupDate = reader.GetDateTime(COL_BackupDate),
                RestoreDate = reader.GetNullableDateTime(COL_RestoreDate),
                BackupByUserId = reader.GetInt(COL_BackupByUserID),
                RestoredByByUserId = reader.GetNullableInt(COL_BackupByUserID)
            };
        }

        #endregion
    }
}
