using CDRComparison.Data;
using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;

namespace CDRComparison.Business
{
    public class CDRSourceConfigManager
    {
        #region Public Methods

        public IEnumerable<CDRSourceConfig> GetCDRSourceConfigs(CDRSourceConfigFilter filter)
        {
            if (filter == null)
                throw new ArgumentNullException("filter");
            Dictionary<int, CDRSourceConfig> cachedCDRSourceConfigs = GetCachedCDRSourceConfigs();
            return cachedCDRSourceConfigs.FindAllRecords(itm => itm.IsPartnerCDRSource == filter.IsPartnerCDRSource);
        }

        public CDRSourceConfig GetCDRSourceConfig(int cdrSourceConfigId)
        {
            Dictionary<int, CDRSourceConfig> cachedCDRSourceConfigs = GetCachedCDRSourceConfigs();
            return cachedCDRSourceConfigs.GetRecord(cdrSourceConfigId);
        }

        public Vanrise.Entities.InsertOperationOutput<CDRSourceConfig> AddCDRSourceConfig(CDRSourceConfig cdrSourceConfig)
        {
            InsertOperationOutput<CDRSourceConfig> output = new Vanrise.Entities.InsertOperationOutput<CDRSourceConfig>();

            output.Result = Vanrise.Entities.InsertOperationResult.Failed;
            output.InsertedObject = null;

            int cdrSourceConfigId = -1;

            ICDRSourceConfigDataManager dataManager = CDRComparisonDataManagerFactory.GetDataManager<ICDRSourceConfigDataManager>();
            bool inserted = dataManager.InsertCDRSourceConfig(cdrSourceConfig, out cdrSourceConfigId);

            if (inserted)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                output.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                cdrSourceConfig.CDRSourceConfigId = cdrSourceConfigId;
                output.InsertedObject = cdrSourceConfig;
            }
            else
            {
                output.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return output;
        }

        #endregion

        #region Private Classes

        class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICDRSourceConfigDataManager _dataManager = CDRComparisonDataManagerFactory.GetDataManager<ICDRSourceConfigDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreCDRSourceConfigsUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Private Methods

        private Dictionary<int, CDRSourceConfig> GetCachedCDRSourceConfigs()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetUsers", () =>
            {
                ICDRSourceConfigDataManager dataManager = CDRComparisonDataManagerFactory.GetDataManager<ICDRSourceConfigDataManager>();
                IEnumerable<CDRSourceConfig> cdrSourceConfigs = dataManager.GetCDRSourceConfigs();
                return cdrSourceConfigs.ToDictionary(kvp => kvp.CDRSourceConfigId, kvp => kvp);
            });
        }

        #endregion
    }
}
