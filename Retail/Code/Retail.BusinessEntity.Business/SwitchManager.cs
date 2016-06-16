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
        #region ctor/Local Variables

        #endregion

        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<SwitchDetail> GetFilteredSwitches(Vanrise.Entities.DataRetrievalInput<SwitchQuery> input)
        {
            Dictionary<int, Switch> cachedSwitches = this.GetCachedSwitches();

            Func<Switch, bool> filterExpression = (switchItem) =>
                (input.Query.Name == null || switchItem.Name.ToLower().Contains(input.Query.Name.ToLower()));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, cachedSwitches.ToBigResult(input, filterExpression, SwitchDetailMapper));
        }
        
        public IEnumerable<SwitchIntegrationConfig> GetSwitchSettingsTemplateConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<SwitchIntegrationConfig>(SwitchIntegrationConfig.EXTENSION_TYPE);
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
            return new SwitchDetail()
            {
                Entity = switchItem
            };
        }

        #endregion

    }
}
