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

        public Vanrise.Entities.IDataRetrievalResult<StateBackupDetail> GetFilteredStateBackups(Vanrise.Entities.DataRetrievalInput<StateBackupQuery> input)
        {
            ExtensionConfigurationManager extensionConfigurationManager = new ExtensionConfigurationManager();
            Dictionary<Guid, StateBackupTypeConfig> backupTypeConfigurations = extensionConfigurationManager.GetExtensionConfigurationsByType<StateBackupTypeConfig>(StateBackupTypeConfig.EXTENSION_TYPE);

            return BigDataManager.Instance.RetrieveData(input, new StateBackupRequestHandler()
                {
                    BackupTypeFilterObject = input.Query.BackupTypeFilterObject,
                    BackupTypeConfigurations = backupTypeConfigurations
                }
                );
        }

        public IEnumerable<StateBackupTypeConfig> GetStateBackupTypes()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<StateBackupTypeConfig>(StateBackupTypeConfig.EXTENSION_TYPE);
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

        private class StateBackupRequestHandler : BigDataRequestHandler<StateBackupQuery, StateBackup, StateBackupDetail>
        {
            public object BackupTypeFilterObject { get; set; }

            public Dictionary<Guid, StateBackupTypeConfig> BackupTypeConfigurations { get; set; }

            public override StateBackupDetail EntityDetailMapper(StateBackup entity)
            {
                StateBackupTypeConfig config = this.BackupTypeConfigurations[entity.Info.ConfigId];
                StateBackupContext context = new StateBackupContext() { Data = entity.Info };

                return new StateBackupDetail()
                {
                    Entity = entity,
                    Description = config.Behavior.GetDescription(context),
                    Type = config.Title
                };
            }

            public override IEnumerable<StateBackup> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<StateBackupQuery> input)
            {
                IStateBackupDataManager dataManager = BEDataManagerFactory.GetDataManager<IStateBackupDataManager>();
                IEnumerable<StateBackup> allEntities = dataManager.GetFilteredStateBackups(input.Query);

                List<StateBackup> filteredResult = new List<StateBackup>();

                foreach (StateBackup entity in allEntities)
                {
                    StateBackupTypeConfig config = this.BackupTypeConfigurations[entity.Info.ConfigId];
                    StateBackupContext context = new StateBackupContext() { Data = entity.Info };
                    if (config.Behavior.IsMatch(context, BackupTypeFilterObject))
                        filteredResult.Add(entity);
                }

                return filteredResult;
            }
        }

        #endregion
    }
}
