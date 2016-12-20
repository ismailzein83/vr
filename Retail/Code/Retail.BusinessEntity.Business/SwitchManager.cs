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
using Vanrise.GenericData.Entities;

namespace Retail.BusinessEntity.Business
{
    public class SwitchManager
    {
        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<SwitchDetail> GetFilteredSwitches(Vanrise.Entities.DataRetrievalInput<SwitchQuery> input)
        {
            Dictionary<int, Switch> cachedSwitches = this.GetCachedSwitches();

            Func<Switch, bool> filterExpression = (switchItem) =>
                (input.Query.Name == null || switchItem.Name.ToLower().Contains(input.Query.Name.ToLower())) &&
                (input.Query.Description == null ||
                    (switchItem.Settings.Description != null && switchItem.Settings.Description.ToLower().Contains(input.Query.Description.ToLower())));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, cachedSwitches.ToBigResult(input, filterExpression, SwitchDetailMapper));
        }

        public IEnumerable<SwitchIntegrationConfig> GetSwitchSettingsTemplateConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<SwitchIntegrationConfig>(SwitchIntegrationConfig.EXTENSION_TYPE);
        }

        public Switch GetSwitch(int switchId)
        {
            Dictionary<int, Switch> cachedSwitches = this.GetCachedSwitches();
            return cachedSwitches.GetRecord(switchId);
        }

        public Vanrise.Entities.InsertOperationOutput<SwitchDetail> AddSwitch(Switch switchItem)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<SwitchDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            ISwitchDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            int switchId = -1;

            if (dataManager.Insert(switchItem, out switchId))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                switchItem.SwitchId = switchId;
                insertOperationOutput.InsertedObject = SwitchDetailMapper(switchItem);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<SwitchDetail> UpdateSwitch(Switch switchItem)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<SwitchDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            ISwitchDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchDataManager>();

            if (dataManager.Update(switchItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = SwitchDetailMapper(this.GetSwitch(switchItem.SwitchId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISwitchDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISwitchDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreSwitchUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Private Methods
        
        Dictionary<int, Switch> GetCachedSwitches()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSwitches", () =>
            {
                ISwitchDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchDataManager>();
                IEnumerable<Switch> switches = dataManager.GetSwitches();
                return switches.ToDictionary(kvp => kvp.SwitchId, kvp => kvp);
            });
        }
        
        #endregion

        #region Mappers

        private SwitchDetail SwitchDetailMapper(Switch switchItem)
        {
            if (switchItem.Settings == null)
                throw new NullReferenceException("switch.Settings");
            if (switchItem.Settings.SwitchIntegration == null)
                throw new NullReferenceException("switch.Settings.SwitchIntegration");
            return new SwitchDetail()
            {
                Entity = switchItem,
                SwitchSettingsTypeName = this.GetSwitchSettingsTypeName(switchItem.Settings.SwitchIntegration.ConfigId)
            };
        }

        private string GetSwitchSettingsTypeName(Guid configId)
        {
            IEnumerable<ExtensionConfiguration> settingsConfigs = this.GetSwitchSettingsTemplateConfigs();
            if (settingsConfigs == null)
                throw new NullReferenceException("settingsConfigs");
            ExtensionConfiguration settingsConfig = settingsConfigs.FindRecord(x => x.ExtensionConfigurationId == configId);
            if (settingsConfig == null)
                throw new NullReferenceException("settingsConfig");
            return settingsConfig.Title;
        }

        #endregion
    }
}
