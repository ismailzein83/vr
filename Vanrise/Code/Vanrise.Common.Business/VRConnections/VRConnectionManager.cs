using System;
using System.Linq;
using System.Collections.Generic;
using Vanrise.Common.Data;
using Vanrise.Entities;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRConnectionManager
    {
        public VRConnection GetVRConnection(Guid vrConnectionId)
        {
            var vrConnections = GetCachedVRConnections();
            if (vrConnections == null)
                return null;

            return vrConnections.GetRecord(vrConnectionId);
        }

        public IEnumerable<VRConnectionInfo> GetVRConnectionInfos(VRConnectionFilter filter)
        {
            var vrConnections = GetCachedVRConnections();
            if (vrConnections == null)
                return null;

            Func<VRConnection, bool> predicate = (itm) =>
            {
                if (filter == null)
                    return true;

                if (filter.Filters != null)
                {
                    foreach (var connectionFilter in filter.Filters)
                        if (!connectionFilter.IsMatched(itm))
                            return false;
                }

                return true;
            };

            var matchedVRConnections = vrConnections.FindAllRecords(predicate);
            if (matchedVRConnections == null)
                return null;

            return matchedVRConnections.MapRecords(VRConnectionInfoMapper);
        }

        public IEnumerable<VRConnection> GetVRConnections<T>() where T : VRConnectionSettings
        {
            var vrConnections = GetCachedVRConnections();
            if (vrConnections == null)
                return null;

            Func<VRConnection, bool> predicate = (itm) =>
            {
                if (itm.Settings == null || itm.Settings as T == null)
                    return false;

                return true;
            };
            return vrConnections.FindAllRecords(predicate);
        }
        public IEnumerable<VRConnectionConfig> GetVRConnectionConfigTypes()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<VRConnectionConfig>(VRConnectionConfig.EXTENSION_TYPE);
        }
        public IEnumerable<VRConnection> GetAllVRConnections()
        {
            return this.GetCachedVRConnections().MapRecords(x => x).OrderBy(x => x.Name);
        }


        public Dictionary<Guid, VRConnection> GetCachedVRConnections()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedVRConnections",
               () =>
               {
                   IVRConnectionDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRConnectionDataManager>();
                   IEnumerable<VRConnection> vrConnections = dataManager.GetVRConnections();
                   return vrConnections.ToDictionary(itm => itm.VRConnectionId, itm => itm);
               });
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IVRConnectionDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IVRConnectionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreVRConnectionsUpdated(ref _updateHandle);
            }
        }

        private VRConnectionInfo VRConnectionInfoMapper(VRConnection vrConnection)
        {
            return new VRConnectionInfo()
            {
                Name = vrConnection.Name,
                VRConnectionId = vrConnection.VRConnectionId
            };
        }
    }
}
