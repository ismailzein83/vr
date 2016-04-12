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
using Vanrise.Security.Business;

namespace CDRComparison.Business
{
    public class CDRSourceConfigManager
    {
        #region Fields

        int _loggedInUserId = new SecurityContext().GetLoggedInUserId();

        #endregion

        #region Public Methods

        public IEnumerable<CDRSourceConfig> GetCDRSourceConfigs(CDRSourceConfigFilter filter)
        {
            if (filter == null)
                throw new ArgumentNullException("filter");
            Dictionary<int, CDRSourceConfig> cachedCDRSourceConfigs = GetCachedCDRSourceConfigs();
            return cachedCDRSourceConfigs.FindAllRecords(itm => itm.IsPartnerCDRSource == filter.IsPartnerCDRSource && itm.UserId == _loggedInUserId);
        }

        public CDRSourceConfig GetCDRSourceConfig(int cdrSourceConfigId)
        {
            Dictionary<int, CDRSourceConfig> cachedCDRSourceConfigs = GetCachedCDRSourceConfigs();
            CDRSourceConfig cdrSourceConfig = cachedCDRSourceConfigs.GetRecord(cdrSourceConfigId);
            return (cdrSourceConfig != null && cdrSourceConfig.UserId == _loggedInUserId) ? cdrSourceConfig : null;
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
