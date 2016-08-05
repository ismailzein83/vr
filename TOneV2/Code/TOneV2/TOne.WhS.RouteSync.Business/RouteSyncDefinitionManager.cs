using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Data;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Common;

namespace TOne.WhS.RouteSync.Business
{
    public class RouteSyncDefinitionManager : IRouteSyncDefinitionManager
    {
        public RouteSyncBPDefinitionSettings GetRouteSyncBPDefinitionSettings(int routeSyncDefinitionId)
        {
            throw new NotImplementedException();
        }

        public RouteSyncDefinition GetRouteSyncDefinitionById(int routeSyncDefinitionId)
        {
            RouteSyncDefinition routeSyncDefinition;
            var allRouteSyncDefinitions = GetCachedRouteSyncDefinitions();
            if (!allRouteSyncDefinitions.TryGetValue(routeSyncDefinitionId, out routeSyncDefinition))
                throw new NullReferenceException(string.Format("GetRouteSyncDefinitionById : {0}", routeSyncDefinitionId));
            return allRouteSyncDefinitions[routeSyncDefinitionId];
        }

        Dictionary<int, RouteSyncDefinition> GetCachedRouteSyncDefinitions()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetRouteSyncDefinitions",
               () =>
               {
                   IRouteSyncDefinitionDataManager dataManager = RouteSyncDataManagerFactory.GetDataManager<IRouteSyncDefinitionDataManager>();
                   IEnumerable<RouteSyncDefinition> routeSyncDefinitions = dataManager.GetRouteSyncDefinitions();
                   return routeSyncDefinitions.ToDictionary(rsd => rsd.RouteSyncDefinitionId, rsd => rsd);
               });
        }

        #region Private Classes

        class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IRouteSyncDefinitionDataManager _dataManager = RouteSyncDataManagerFactory.GetDataManager<IRouteSyncDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreRouteSyncDefinitionsUpdated(ref _updateHandle);
            }
        }

        #endregion


    }
}
