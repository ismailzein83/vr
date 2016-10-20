using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class StateBackupDataManager : IStateBackupDataManager
    {
        #region Private Memebers

        private StateBackupTypeBehavior _stateBackupBehavior = null;

        #endregion

        #region Public Methods

        public void BackupData(StateBackupType backupType)
        {
            this.PrepareData(backupType);
            //Save Backup Record to DB and get StateBackup Id to pass it to the next method
            string backupCommand = _stateBackupBehavior.GetBackupCommands(1);
        }

        public void RestoreData(int stateBackupId)
        {
            //Get State Backup from DB
            //Call prepare data sending it the backup type
            //Call restore
        }

        #endregion

        #region Private Methods

        private void PrepareData(StateBackupType backupType)
        {
            if (backupType is StateBackupAllCustomers)
                _stateBackupBehavior = new StateBackupAllCustomersBehavior();
            else if (backupType is StateBackupCustomer)
                _stateBackupBehavior = new StateBackupCustomerBehavior();

            if (_stateBackupBehavior == null)
                throw new InvalidOperationException("Backup Type is not specified");

            _stateBackupBehavior.Data = backupType;
            _stateBackupBehavior.BackupDatabaseName = System.Configuration.ConfigurationManager.AppSettings["StateBackupDatabase"];
        }

        #endregion
    }
}
