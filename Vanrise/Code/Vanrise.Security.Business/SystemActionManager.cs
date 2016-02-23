using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class SystemActionManager
    {
        #region Public Methods

        public IEnumerable<SystemAction> GetSystemActions()
        {
            return GetCachedSystemActions().Values;
        }

        #endregion

        #region Private Methods

        Dictionary<int, SystemAction> GetCachedSystemActions()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("", () =>
               {
                   ISystemActionDataManager dataManager = SecurityDataManagerFactory.GetDataManager<ISystemActionDataManager>();
                   IEnumerable<SystemAction> systemActions = dataManager.GetSystemActions();
                   return systemActions.ToDictionary(kvp => kvp.SystemActionId, kvp => kvp);
               });
        }
        
        #endregion

        #region Private Classes

        class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISystemActionDataManager _dataManager = SecurityDataManagerFactory.GetDataManager<ISystemActionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreSystemActionsUpdated(ref _updateHandle);
            }
        }
        
        #endregion
    }
}
