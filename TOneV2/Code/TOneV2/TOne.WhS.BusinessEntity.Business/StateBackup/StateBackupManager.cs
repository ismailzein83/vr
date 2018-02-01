using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Security.Business;

namespace TOne.WhS.BusinessEntity.Business
{
    public class StateBackupManager
    {
        #region Public Methods


        public Vanrise.Entities.IDataRetrievalResult<StateBackupDetail> GetFilteredStateBackups(Vanrise.Entities.DataRetrievalInput<StateBackupQuery> input)
        {
            ExtensionConfigurationManager extensionConfigurationManager = new ExtensionConfigurationManager();
            Dictionary<Guid, StateBackupTypeConfig> backupTypeConfigurations = extensionConfigurationManager.GetExtensionConfigurationsByType<StateBackupTypeConfig>(StateBackupTypeConfig.EXTENSION_TYPE);

            UserManager userManager = new UserManager();

            return BigDataManager.Instance.RetrieveData(input, new StateBackupRequestHandler()
                {
                    BackupTypeFilterObject = input.Query.BackupTypeFilterObject,
                    BackupTypeConfigurations = backupTypeConfigurations,
                    UserManager = userManager
                }
                );
        }

        public IEnumerable<StateBackup> GetStateBackupsAfterId(long stateBackupId)
        {
            IStateBackupDataManager manager = BEDataManagerFactory.GetDataManager<IStateBackupDataManager>();
            return manager.GetStateBackupsAfterId(stateBackupId);
        }
        public IEnumerable<StateBackupTypeConfig> GetStateBackupTypes()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<StateBackupTypeConfig>(StateBackupTypeConfig.EXTENSION_TYPE);
        }

        public object BackupData(StateBackupType backupType)
        {
            IStateBackupDataManager manager = BEDataManagerFactory.GetDataManager<IStateBackupDataManager>();
            return manager.BackupData(backupType);
        }

        public UpdateOperationOutput<StateBackup> RestoreData(long stateBackupId)
        {
            UpdateOperationOutput<StateBackup> updateOperationOutput = new UpdateOperationOutput<StateBackup>
            {
                Result = UpdateOperationResult.Failed,
                UpdatedObject = null
            };
            IStateBackupDataManager dataManager = BEDataManagerFactory.GetDataManager<IStateBackupDataManager>();
            StateBackup currentStateBackup = dataManager.GetStateBackup(stateBackupId);

            ExtensionConfigurationManager extensionConfigurationManager = new ExtensionConfigurationManager();
            Dictionary<Guid, StateBackupTypeConfig> backupTypeConfigurations = extensionConfigurationManager.GetExtensionConfigurationsByType<StateBackupTypeConfig>(StateBackupTypeConfig.EXTENSION_TYPE);

            StateBackupTypeConfig config = backupTypeConfigurations[currentStateBackup.Info.ConfigId];

            StateBackupCanRestoreContext stateBackupCanRestorecontext = new StateBackupCanRestoreContext
            {
                StateBackupId = stateBackupId,
                StateBackupType = currentStateBackup.Info

            };
            if (!config.Behavior.CanRestore(stateBackupCanRestorecontext))
            {
                updateOperationOutput.ShowExactMessage = true;
                updateOperationOutput.Message = stateBackupCanRestorecontext.ErrorMessage;
                return updateOperationOutput;
            }
            // on each restore we have to backup the data (to handle failure cases)
            //in case this backup is a backup from restore, we don't back it up again
            if (currentStateBackup.Info.OnRestoreStateBackupId == null)
            {
                currentStateBackup.Info.OnRestoreStateBackupId = stateBackupId;
                BackupData(currentStateBackup.Info);
            }

            SecurityContext securityContext = new SecurityContext();
            bool updateActionSucc = dataManager.RestoreData(stateBackupId, currentStateBackup.Info, securityContext.GetLoggedInUserId());
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

            public UserManager UserManager { get; set; }

            public override StateBackupDetail EntityDetailMapper(StateBackup entity)
            {
                StateBackupTypeConfig config = this.BackupTypeConfigurations[entity.Info.ConfigId];
                StateBackupContext context = new StateBackupContext() { Data = entity.Info };

                return new StateBackupDetail()
                {
                    Entity = entity,
                    Description = config.Behavior.GetDescription(context),
                    Type = config.Title,
                    BackupByUsername = UserManager.GetUserName(entity.BackupByUserId),
                    RestoredByUsername = entity.RestoredByByUserId.HasValue ? UserManager.GetUserName(entity.RestoredByByUserId.Value) : null
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

                    if (BackupTypeFilterObject == null || config.Behavior.IsMatch(context, BackupTypeFilterObject))
                        filteredResult.Add(entity);
                }

                return filteredResult;
            }
        }

        #endregion
    }
}
