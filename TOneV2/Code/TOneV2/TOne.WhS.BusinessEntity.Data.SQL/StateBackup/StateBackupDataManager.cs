using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class StateBackupDataManager : BaseSQLDataManager, IStateBackupDataManager
    {

        #region ctor/Local Variables
        public StateBackupDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }
        #endregion

        #region Private Memebers

        private StateBackupTypeBehavior _stateBackupBehavior = null;

        #endregion

        #region Public Methods

        public IEnumerable<StateBackup> GetFilteredStateBackups(StateBackupQuery input)
        {
            return GetItemsSP("[TOneWhS_BE].[sp_StateBackup_GetFiltered]", StateBackupMapper, input.BackupTypeFilterConfigId, input.From, input.To);
        }

        public object BackupData(StateBackupType backupType)
        {
            this.PrepareData(backupType);

            var options = new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
                Timeout = TransactionManager.MaximumTimeout
            };

            object stateBackupId;

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                ExecuteNonQuerySP("TOneWhS_BE.sp_StateBackup_Insert", out stateBackupId, backupType.ConfigId, Vanrise.Common.Serializer.Serialize(backupType), DateTime.Now, backupType.UserId);
                string backupCommand = _stateBackupBehavior.GetBackupCommands((long)stateBackupId);
                ExecuteNonQueryText(backupCommand, null);
                scope.Complete();
            }

            return stateBackupId;
        }

        public IEnumerable<StateBackup> GetStateBackupsAfterId(long stateBackupId)
        {
            return GetItemsSP("[TOneWhS_BE].[sp_StateBackup_GetStateBackupsAfterId]", StateBackupMapper, stateBackupId);
        }

        public bool RestoreData(long stateBackupId, StateBackupType info, int userId)
        {
            this.PrepareData(info);
            bool result = false;
            var options = new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
                Timeout = TransactionManager.MaximumTimeout
            };

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                string restoreCommand = _stateBackupBehavior.GetRestoreCommands(stateBackupId);
                ExecuteNonQueryText(restoreCommand, null);

                ExecuteNonQuerySP("TOneWhS_BE.sp_StateBackup_Update", stateBackupId, DateTime.Now, userId);

                scope.Complete();
                result = true;

            }
            return result;
        }


        public StateBackup GetStateBackup(long stateBackupId)
        {
            return GetItemSP("TOneWhS_BE.sp_StateBackup_GetById", StateBackupMapper, stateBackupId);
        }

        #endregion

        #region Private Methods

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

        private StateBackup StateBackupMapper(IDataReader reader)
        {
            StateBackup stateBackup = new StateBackup
           {
               StateBackupId = (long)reader["ID"],
               BackupTypeConfigId = GetReaderValue<Guid>(reader, "ConfigID"),
               Info = Vanrise.Common.Serializer.Deserialize<StateBackupType>(reader["Info"] as string),
               BackupDate = GetReaderValue<DateTime>(reader, "BackupDate"),
               RestoreDate = GetReaderValue<DateTime?>(reader, "RestoreDate"),
               BackupByUserId = (int)reader["BackupByUserID"],
               RestoredByByUserId = GetReaderValue<int?>(reader, "RestoredByUserID")
           };

            return stateBackup;
        }

        #endregion
    }
}
