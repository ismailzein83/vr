using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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



        public void BackupData(StateBackupType backupType)
        {
            this.PrepareData(backupType);
            object stateBackupId;
            ExecuteNonQuerySP("TOneWhS_BE.sp_StateBackup_Insert",out stateBackupId, "Descriptions", Vanrise.Common.Serializer.Serialize(backupType), DateTime.Now);
            string backupCommand = _stateBackupBehavior.GetBackupCommands((int)stateBackupId);

            ExecuteNonQueryText(backupCommand, null);
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
            else if (backupType is StateBackupSupplier)
                _stateBackupBehavior = new StateBackupSupplierBehavior();

            if (_stateBackupBehavior == null)
                throw new InvalidOperationException("Backup Type is not specified");

            _stateBackupBehavior.Data = backupType;
            _stateBackupBehavior.BackupDatabaseName = System.Configuration.ConfigurationManager.AppSettings["StateBackupDatabase"];
        }

        #endregion
    }
}
