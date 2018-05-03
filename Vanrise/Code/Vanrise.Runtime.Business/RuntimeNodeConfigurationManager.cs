using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime;
using Vanrise.Runtime.Data;
using Vanrise.Runtime.Entities;
using Vanrise.Runtime.Entities.Configuration;
using Vanrise.Common;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Runtime.Business
{
    public class RuntimeNodeConfigurationManager
    {
        static IRuntimeNodeConfigurationDataManager s_dataManager = RuntimeDataManagerFactory.GetDataManager<IRuntimeNodeConfigurationDataManager>();

        #region Public Methods
        public Dictionary<Guid, RuntimeNodeConfiguration> GetAllNodeConfigurations()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
           .GetOrCreateObject("GetCacheRuntimeNodesConfigurations", () =>
           {
               var allNodeConfigs = s_dataManager.GetAllNodeConfigurations();
               return allNodeConfigs != null ? allNodeConfigs.ToDictionary(n => n.RuntimeNodeConfigurationId, n => n) : null;
           });
        }

        public RuntimeNodeConfiguration GetNodeConfiguration(Guid nodeConfigurationId)
        {
            return GetAllNodeConfigurations().GetRecord(nodeConfigurationId);

        }
        public IEnumerable<RuntimeNodeConfigurationDetails> GetRuntimeNodeConfigurationsInfo()
        {
            var allRuntimeNodeConfigurations = GetAllNodeConfigurations();
            Func<RuntimeNodeConfiguration, bool> filterFunc = (runtimeNodeConfiguration) =>
            {
                return true;
            };
            IEnumerable<RuntimeNodeConfiguration> filteredNodeConfigurations = (filterFunc != null) ? allRuntimeNodeConfigurations.FindAllRecords(filterFunc) : allRuntimeNodeConfigurations.MapRecords(u => u.Value);
            return filteredNodeConfigurations.MapRecords(RuntimeNodeConfigurationDetailMapper).OrderBy(runtimeNodeConfiguration => runtimeNodeConfiguration.Name);
        }

        public IDataRetrievalResult<RuntimeNodeConfigurationDetails> GetFilteredRuntimeNodesConfigurations(DataRetrievalInput<RuntimeNodeConfigurationQuery> input)
        {
            var allNodesConfigurations = GetAllNodeConfigurations();
            Func<RuntimeNodeConfiguration, bool> filterExpression = (runtimeNodeConfiguration) =>
            {
                if (input.Query.Name != null && !runtimeNodeConfiguration.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                return true;
            };
            return DataRetrievalManager.Instance.ProcessResult(input, allNodesConfigurations.ToBigResult(input, filterExpression, RuntimeNodeConfigurationDetailMapper));
        }

        public InsertOperationOutput<RuntimeNodeConfigurationDetails> AddRuntimeNodeConfiguration(RuntimeNodeConfiguration runtimeNodeConfiguration)
        {
            IRuntimeNodeConfigurationDataManager runtimeNodeConfigurationDataManager = RuntimeDataManagerFactory.GetDataManager<IRuntimeNodeConfigurationDataManager>();
            InsertOperationOutput<RuntimeNodeConfigurationDetails> insertOperationOutput = new InsertOperationOutput<RuntimeNodeConfigurationDetails>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            runtimeNodeConfiguration.RuntimeNodeConfigurationId = Guid.NewGuid();
            bool insertActionSuccess = runtimeNodeConfigurationDataManager.Insert(runtimeNodeConfiguration);
            if (insertActionSuccess)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = RuntimeNodeConfigurationDetailMapper(runtimeNodeConfiguration);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
        }

        public UpdateOperationOutput<RuntimeNodeConfigurationDetails> UpdateRuntimeNodeConfiguration(RuntimeNodeConfiguration runtimeNodeConfiguration)
        {
            IRuntimeNodeConfigurationDataManager runtimeNodeConfigurationDataManager = RuntimeDataManagerFactory.GetDataManager<IRuntimeNodeConfigurationDataManager>();
            UpdateOperationOutput<RuntimeNodeConfigurationDetails> updateOperationOutput = new UpdateOperationOutput<RuntimeNodeConfigurationDetails>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            bool updateActionSuccess = runtimeNodeConfigurationDataManager.Update(runtimeNodeConfiguration); //insert to update
            if (updateActionSuccess)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = RuntimeNodeConfigurationDetailMapper(runtimeNodeConfiguration);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }
        public IEnumerable<RuntimeServiceConfig> GetRuntimeServiceTypeTemplateConfigs()
        {
            return BusinessManagerFactory.GetManager<IExtensionConfigurationManager>().GetExtensionConfigurations<RuntimeServiceConfig>(RuntimeServiceConfig.EXTENSION_TYPE);
        }

        #endregion

        #region Private Methods
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IRuntimeNodeConfigurationDataManager runtimeNodeConfigurationDataManager = RuntimeDataManagerFactory.GetDataManager<IRuntimeNodeConfigurationDataManager>();
            object _updateHandle;
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return runtimeNodeConfigurationDataManager.AreRuntimeNodeConfigurationUpdated(ref _updateHandle);
            }
        }
        #endregion

        #region Mapper
        private RuntimeNodeConfigurationDetails RuntimeNodeConfigurationDetailMapper(RuntimeNodeConfiguration nodeConfig)
        {
            RuntimeNodeConfigurationDetails runtimeNodeConfigurationDetails = new RuntimeNodeConfigurationDetails();
            runtimeNodeConfigurationDetails.RuntimeNodeConfigurationId = nodeConfig.RuntimeNodeConfigurationId;
            runtimeNodeConfigurationDetails.Name = nodeConfig.Name;
            return runtimeNodeConfigurationDetails;
        }
        #endregion
    }
}
