using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
namespace Retail.BusinessEntity.Business
{
    public class ActionDefinitionManager : IActionDefinitionManager
    {

        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<ActionDefinitionDetail> GetFilteredActionDefinitions(Vanrise.Entities.DataRetrievalInput<ActionDefinitionQuery> input)
        {
            Dictionary<Guid, ActionDefinition> cachedActionDefinitiones = this.GetCachedActionDefinitions();

            Func<ActionDefinition, bool> filterExpression = (actionDefinition) =>
                (input.Query.Name == null || actionDefinition.Name.ToLower().Contains(input.Query.Name.ToLower()));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, cachedActionDefinitiones.ToBigResult(input, filterExpression, ActionDefinitionDetailMapper));
        }
        public ActionDefinition GetActionDefinition(Guid actionDefinitionId)
        {
            Dictionary<Guid, ActionDefinition> cachedActionDefinitiones = this.GetCachedActionDefinitions();
            return cachedActionDefinitiones.GetRecord(actionDefinitionId);
        }
        public Vanrise.Entities.InsertOperationOutput<ActionDefinitionDetail> AddActionDefinition(ActionDefinition actionDefinition)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<ActionDefinitionDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IActionDefinitionDataManager dataManager = BEDataManagerFactory.GetDataManager<IActionDefinitionDataManager>();
            actionDefinition.ActionDefinitionId = Guid.NewGuid();

            if (dataManager.Insert(actionDefinition))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = ActionDefinitionDetailMapper(actionDefinition);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<ActionDefinitionDetail> UpdateActionDefinition(ActionDefinition actionDefinition)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<ActionDefinitionDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IActionDefinitionDataManager dataManager = BEDataManagerFactory.GetDataManager<IActionDefinitionDataManager>();

            if (dataManager.Update(actionDefinition))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = ActionDefinitionDetailMapper(this.GetActionDefinition(actionDefinition.ActionDefinitionId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }
        public IEnumerable<ActionBPDefinitionConfig> GetActionBPDefinitionExtensionConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<ActionBPDefinitionConfig>(ActionBPDefinitionConfig.EXTENSION_TYPE);
        }
        public IEnumerable<ProvisionerDefinitionConfig> GetProvisionerDefinitionExtensionConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<ProvisionerDefinitionConfig>(ProvisionerDefinitionConfig.EXTENSION_TYPE);
        }

        public IEnumerable<ActionDefinitionInfo> GetActionDefinitionInfoByEntityType(EntityType entityType, Guid statusId, Guid? serviceTypeId = null)
        {
            Dictionary<Guid, ActionDefinition> cachedActionDefinitiones = this.GetCachedActionDefinitions();
            Func<ActionDefinition, bool> filterExpression = (item) =>
            {
                if (item.Settings.SupportedOnStatuses == null)
                    throw new NullReferenceException("SupportedOnStatuses is null.");

                if (item.EntityType == entityType && (item.Settings.EntityTypeId == null || item.Settings.EntityTypeId == serviceTypeId) && item.Settings.SupportedOnStatuses.Any(x => x.StatusDefinitionId == statusId))
                    return true;
                return false;
            };
            return cachedActionDefinitiones.MapRecords(ActionDefinitionInfoMapper, filterExpression);
        }

        public IEnumerable<ActionDefinitionInfo> GetActionDefinitionsInfo(ActionDefinitionInfoFilter filter)
        {
            Func<ActionDefinition, bool> filterExpression = null;
            if (filter != null)
            {
            }
            return this.GetCachedActionDefinitions().MapRecords(ActionDefinitionInfoMapper, filterExpression).OrderBy(x => x.Name);
        }

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IActionDefinitionDataManager _dataManager = BEDataManagerFactory.GetDataManager<IActionDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreActionDefinitionUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Private Methods

        public T GetActionBPDefinitionSettings<T>(Guid actionDefinitionId) where T : ActionBPDefinitionSettings
        {
            ActionBPDefinitionSettings bpDefinitionSettings = GetActionBPDefinitionSettings(actionDefinitionId);
            T castedBPDefinitionSettings = bpDefinitionSettings as T;
            if (castedBPDefinitionSettings == null)
                throw new Exception(String.Format("bpDefinitionSettings should be of type '{0}'. it is of type '{1}'", typeof(T), bpDefinitionSettings.GetType()));
            return castedBPDefinitionSettings;
        }

        public ActionBPDefinitionSettings GetActionBPDefinitionSettings(Guid actionDefinitionId)
        {
            var actionDefinitionSettings = GetActionDefinitionSettings(actionDefinitionId);
            if (actionDefinitionSettings.BPDefinitionSettings == null)
                throw new NullReferenceException(String.Format("actionDefinition.Settings.BPDefinitionSettings. Id '{0}'", actionDefinitionId));
            return actionDefinitionSettings.BPDefinitionSettings;
        }
        private ActionDefinitionSettings GetActionDefinitionSettings(Guid actionDefinitionId)
        {
            var actionDefinition = GetActionDefinition(actionDefinitionId);
            if (actionDefinition == null)
                throw new NullReferenceException(String.Format("actionDefinition. Id '{0}'", actionDefinitionId));
            if (actionDefinition.Settings == null)
                throw new NullReferenceException(String.Format("actionDefinition.Settings. Id '{0}'", actionDefinitionId));
            return actionDefinition.Settings;
        }
        Dictionary<Guid, ActionDefinition> GetCachedActionDefinitions()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetActionDefinitions", () =>
            {
                IActionDefinitionDataManager dataManager = BEDataManagerFactory.GetDataManager<IActionDefinitionDataManager>();
                IEnumerable<ActionDefinition> actionDefinitiones = dataManager.GetActionDefinitions();
                return actionDefinitiones.ToDictionary(kvp => kvp.ActionDefinitionId, kvp => kvp);
            });
        }
        #endregion

        #region Mappers
        private ActionDefinitionDetail ActionDefinitionDetailMapper(ActionDefinition actionDefinition)
        {
            return new ActionDefinitionDetail()
            {
                Entity = actionDefinition,
                EntityTypeDescription = Utilities.GetEnumDescription<EntityType>(actionDefinition.EntityType)
            };
        }
        private ActionDefinitionInfo ActionDefinitionInfoMapper(ActionDefinition actionDefinition)
        {
            string fullName = null;
            switch(actionDefinition.EntityType)
            {
                case EntityType.Account:
                    fullName = string.Format("{0} {1}", actionDefinition.Name, Utilities.GetEnumDescription(actionDefinition.EntityType));
                    break;
                case EntityType.AccountService:
                 ServiceTypeManager manager = new ServiceTypeManager();
                    var serviceEntity = manager.GetServiceType(actionDefinition.Settings.EntityTypeId.Value);
                    fullName= string.Format("{0} {1}", actionDefinition.Name, serviceEntity.Name);
                    break;
            }
            return new ActionDefinitionInfo()
            {
                ActionDefinitionId = actionDefinition.ActionDefinitionId,
                Name = actionDefinition.Name,
                FullName = fullName
            };
        }
        #endregion
    }
}
