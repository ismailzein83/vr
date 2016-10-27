using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.BusinessEntity.Business
{
    public class StateBackupManager
    {
        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<StateBackup> GetFilteredStateBackups(Vanrise.Entities.DataRetrievalInput<StateBackupQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new StateBackupRequestHandler());
        }

        public void BackupData(StateBackupType backupType)
        {
            IStateBackupDataManager manager = BEDataManagerFactory.GetDataManager<IStateBackupDataManager>();
            manager.BackupData(backupType);
        }

        public TOne.Entities.UpdateOperationOutput<StateBackup> RestoreData(long stateBackupId)
        {
            TOne.Entities.UpdateOperationOutput<StateBackup> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<StateBackup>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IStateBackupDataManager dataManager = BEDataManagerFactory.GetDataManager<IStateBackupDataManager>();
            bool updateActionSucc = dataManager.RestoreData(stateBackupId);
            if (updateActionSucc)
            {
                StateBackup stateBackup = dataManager.GetStateBackup(stateBackupId);
                updateOperationOutput.Message = "Backup Restored Successfully";
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = stateBackup;
            }
            else
            {
                updateOperationOutput.Message = "An Error Occured";
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            }
                

            return updateOperationOutput;
        }


        #endregion

        #region Private Classes

        private class StateBackupRequestHandler : BigDataRequestHandler<StateBackupQuery, StateBackup, StateBackup>
        {
            public override StateBackup EntityDetailMapper(StateBackup entity)
            {
                StateBackupManager manager = new StateBackupManager();
                return manager.StateBackupMapper(entity);
            }

            public override IEnumerable<StateBackup> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<StateBackupQuery> input)
            {
                IStateBackupDataManager dataManager = BEDataManagerFactory.GetDataManager<IStateBackupDataManager>();
                return dataManager.GetFilteredStateBackups(input.Query);
            }
        }

        #endregion

        #region Private Mappers
        private StateBackup StateBackupMapper(StateBackup stateBackup)
        {
            return stateBackup;
        }

        #endregion

    }
}
