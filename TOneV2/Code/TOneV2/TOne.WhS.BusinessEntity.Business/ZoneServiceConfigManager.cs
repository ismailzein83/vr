using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Caching;
using TOne.WhS.BusinessEntity.Data;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class ZoneServiceConfigManager
    {

        #region ctor/Local Variables
        #endregion

        #region Public Methods

        public IEnumerable<ZoneServiceConfig> GetAllServices()
        {
            var allServices = GetCachedZoneServiceConfigs();
            if (allServices == null)
                return null;

            return allServices.Values;
        }


        public IDataRetrievalResult<ZoneServiceConfigDetail> GetFilteredZoneServiceConfigs(DataRetrievalInput<ZoneServiceConfigQuery> input)
        {
            var allZoneServiceConfigs = GetCachedZoneServiceConfigs();

            Func<ZoneServiceConfig, bool> filterExpression = (x) =>
            {
                if (!string.IsNullOrEmpty(input.Query.Name) && !x.Settings.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                return true;
            };


            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allZoneServiceConfigs.ToBigResult(input, filterExpression, ZoneServiceConfigDetailMapper));
        }
        public Dictionary<int, ZoneServiceConfig> GetCachedZoneServiceConfigs()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetZoneServiceConfigs",
               () =>
               {
                   IZoneServiceConfigDataManager dataManager = BEDataManagerFactory.GetDataManager<IZoneServiceConfigDataManager>();
                   IEnumerable<ZoneServiceConfig> rateTypes = dataManager.GetZoneServiceConfigs();
                   return rateTypes.ToDictionary(x => x.ZoneServiceConfigId, x => x);
               });
        }


        public IEnumerable<ZoneServiceConfigInfo> GetAllZoneServiceConfigs(ZoneServiceConfigFilter filter)
        {
            var allZoneServiceConfigs = GetCachedZoneServiceConfigs();
            if (allZoneServiceConfigs == null)
                return null;

            Func<ZoneServiceConfig, bool> filterExpression = null;

            if (filter != null)
            {
                List<int> childrenServiceIds = new List<int>();
                if (filter.AssinableToServiceId != null)
                    this.GetChildServices(filter.AssinableToServiceId.Value, childrenServiceIds);

                filterExpression = (prod) =>
                {
                    if (filter.AssinableToServiceId != null && filter.AssinableToServiceId == prod.ZoneServiceConfigId)
                        return false;
                    if (childrenServiceIds.Count > 0 && childrenServiceIds.Contains(prod.ZoneServiceConfigId))
                        return false;

                    return true;
                };
            }

            return allZoneServiceConfigs.FindAllRecords(filterExpression).MapRecords(ZoneServiceConfigInfoMapper);
        }

        public IEnumerable<ZoneServiceConfig> GetAllZoneServices()
        {
            var allZoneServiceConfigs = GetCachedZoneServiceConfigs();
            if (allZoneServiceConfigs == null)
                return null;
            return allZoneServiceConfigs.Values;
        }

        public IEnumerable<ZoneServiceConfig> GetAllZoneServicesByIds(HashSet<int> serviceIds)
        {
            if (serviceIds == null)
                return null;

            var allZoneServiceConfigs = GetCachedZoneServiceConfigs();
            if (allZoneServiceConfigs == null)
                return null;

            Func<ZoneServiceConfig, bool> filterExpression = (itm) => serviceIds.Contains(itm.ZoneServiceConfigId);
            return allZoneServiceConfigs.FindAllRecords(filterExpression);
        }

        public string GetZoneServicesNames(List<int> services)
        {
            var allZoneServiceConfigs = GetCachedZoneServiceConfigs().FindAllRecords(x=>services.Contains(x.Key));
            if (allZoneServiceConfigs == null)
                return null;
            return String.Join(",", allZoneServiceConfigs.Select(x => x.Value.Symbol).ToList());
        }

        public Dictionary<int, string> GetZoneServicesNamesDict(IEnumerable<int> services)
        {
            var allZoneServiceConfigs = GetCachedZoneServiceConfigs().FindAllRecords(x => services.Contains(x.Key));
            if (allZoneServiceConfigs == null)
                return null;

            return allZoneServiceConfigs.ToDictionary(x => x.Key, x => x.Value.Symbol);
        }

        public ZoneServiceConfig GetZoneServiceConfig(int ZoneServiceConfigId)
        {
            var allZoneServiceConfigs = GetCachedZoneServiceConfigs();
            return allZoneServiceConfigs.GetRecord(ZoneServiceConfigId);
        }

        public string GetServiceSymbol(int serviceId)
        {
            ZoneServiceConfig zoneServiceConfig = GetZoneServiceConfig(serviceId);

            if (zoneServiceConfig == null)
                throw new NullReferenceException(string.Format("zoneServiceConfig Id: {0} does not exist", serviceId));

            return zoneServiceConfig.Symbol;
        }

        public IEnumerable<ZoneServiceConfig> GetDistinctZoneServiceConfigsWithChildren(IEnumerable<int> zoneServiceIds, bool getOnlyDirectChildren = false)
        {
            if (zoneServiceIds == null || zoneServiceIds.Count() == 0)
                return null;

            Dictionary<int, Dictionary<int, ZoneServiceConfig>> result = GetAllZoneServiceConfigsWithChildren(getOnlyDirectChildren);
            Dictionary<int, ZoneServiceConfig> zoneServiceConfigs = new Dictionary<int, ZoneServiceConfig>();
            ZoneServiceConfig tempZoneServiceConfig;

            foreach (int zoneServiceId in zoneServiceIds)
            {
                Dictionary<int, ZoneServiceConfig> zoneServiceChildren = result.GetRecord(zoneServiceId);
                if (zoneServiceChildren == null || zoneServiceChildren.Count == 0)
                    continue;
                foreach (KeyValuePair<int, ZoneServiceConfig> zoneServiceChild in zoneServiceChildren)
                {
                    if (!zoneServiceConfigs.TryGetValue(zoneServiceChild.Key, out tempZoneServiceConfig))
                    {
                        zoneServiceConfigs.Add(zoneServiceChild.Key, zoneServiceChild.Value);
                    }
                }
            }
            return zoneServiceConfigs.Values;
        }

        private Dictionary<int, Dictionary<int, ZoneServiceConfig>> GetAllZoneServiceConfigsWithChildren(bool getOnlyDirectChildren)
        {
            string cacheName;
            if (getOnlyDirectChildren)
                cacheName = "GetAllZoneServicesWithDirectChildren";
            else
                cacheName = "GetAllZoneServicesWithAllChildren";

            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName,
               () =>
               {
                   Dictionary<int, Dictionary<int, ZoneServiceConfig>> result = new Dictionary<int, Dictionary<int, ZoneServiceConfig>>();
                   IEnumerable<ZoneServiceConfig> allServices = GetAllServices();
                   foreach (ZoneServiceConfig zoneServiceConfig in allServices)
                   {
                       Dictionary<int, ZoneServiceConfig> zoneServiceConfigs = new Dictionary<int, ZoneServiceConfig>();
                       zoneServiceConfigs.Add(zoneServiceConfig.ZoneServiceConfigId, zoneServiceConfig);
                       result.Add(zoneServiceConfig.ZoneServiceConfigId, zoneServiceConfigs);
                       BuildZoneServicesWithChildren(new List<int>() { zoneServiceConfig.ZoneServiceConfigId }, zoneServiceConfigs, allServices, getOnlyDirectChildren);
                   }

                   return result;
               });
        }

        public TOne.Entities.InsertOperationOutput<ZoneServiceConfigDetail> AddZoneServiceConfig(ZoneServiceConfig zoneServiceConfig)
        {
            TOne.Entities.InsertOperationOutput<ZoneServiceConfigDetail> insertOperationOutput = new TOne.Entities.InsertOperationOutput<ZoneServiceConfigDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            if (GetCachedZoneServiceConfigs().FindRecord(x => x.Value.Settings.Color.ToLower() == zoneServiceConfig.Settings.Color.ToLower()).Value != null)
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
                return insertOperationOutput;
            }

            insertOperationOutput.InsertedObject = null;
            int ZoneServiceConfigtId = -1;

            IZoneServiceConfigDataManager dataManager = BEDataManagerFactory.GetDataManager<IZoneServiceConfigDataManager>();
            bool insertActionSucc = dataManager.Insert(zoneServiceConfig, out ZoneServiceConfigtId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                zoneServiceConfig.ZoneServiceConfigId = ZoneServiceConfigtId;
                insertOperationOutput.InsertedObject = ZoneServiceConfigDetailMapper(zoneServiceConfig);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        public TOne.Entities.UpdateOperationOutput<ZoneServiceConfigDetail> UpdateZoneServiceConfig(ZoneServiceConfig zoneServiceConfig)
        {
            IZoneServiceConfigDataManager dataManager = BEDataManagerFactory.GetDataManager<IZoneServiceConfigDataManager>();

            TOne.Entities.UpdateOperationOutput<ZoneServiceConfigDetail> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<ZoneServiceConfigDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            if (GetCachedZoneServiceConfigs().FindRecord(x => x.Value.Settings.Color.ToLower() == zoneServiceConfig.Settings.Color.ToLower() && x.Key != zoneServiceConfig.ZoneServiceConfigId).Value != null)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
                return updateOperationOutput;
            }

            bool updateActionSucc = dataManager.Update(zoneServiceConfig);
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = ZoneServiceConfigDetailMapper(zoneServiceConfig);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }
        #endregion

        #region Private Methods

        private void BuildZoneServicesWithChildren(List<int> zoneServiceConfigIds, Dictionary<int, ZoneServiceConfig> zoneServiceConfigs, IEnumerable<ZoneServiceConfig> allServices, bool getOnlyDirectChildren)
        {
            if (zoneServiceConfigIds == null || zoneServiceConfigIds.Count == 0)
                return;

            List<int> childZoneServiceConfigIds = new List<int>();
            foreach (int zoneServiceConfigId in zoneServiceConfigIds)
            {
                IEnumerable<ZoneServiceConfig> childServices = allServices.FindAllRecords(x => x.Settings.ParentId != null && x.Settings.ParentId == zoneServiceConfigId);
                if (childServices != null && childServices.Count() > 0)
                {
                    childZoneServiceConfigIds.AddRange(GetAndAddNonDuplicateZoneServiceConfigIds(childServices, zoneServiceConfigs));
                }
            }
            if (getOnlyDirectChildren)
                return;

            BuildZoneServicesWithChildren(childZoneServiceConfigIds, zoneServiceConfigs, allServices, getOnlyDirectChildren);
        }

        private List<int> GetAndAddNonDuplicateZoneServiceConfigIds(IEnumerable<ZoneServiceConfig> zoneServiceConfigList, Dictionary<int, ZoneServiceConfig> zoneServiceConfigs)
        {
            List<int> newAddedServiceConfigIds = new List<int>();
            foreach (ZoneServiceConfig zoneServiceConfig in zoneServiceConfigList)
            {
                ZoneServiceConfig duplicateZoneServices = zoneServiceConfigs.GetRecord(zoneServiceConfig.ZoneServiceConfigId);
                if (duplicateZoneServices == null)
                {
                    newAddedServiceConfigIds.Add(zoneServiceConfig.ZoneServiceConfigId);
                    zoneServiceConfigs.Add(zoneServiceConfig.ZoneServiceConfigId, zoneServiceConfig);
                }
            }
            return newAddedServiceConfigIds;
        }

        private void GetChildServices(int serviceId, List<int> childrenServiceIds)
        {
            IEnumerable<ZoneServiceConfig> childServices = GetAllServices().FindAllRecords(x => x.Settings.ParentId != null && x.Settings.ParentId == serviceId);
            if (childServices == null)
                return;

            foreach (ZoneServiceConfig child in childServices)
            {
                childrenServiceIds.Add(child.ZoneServiceConfigId);
                GetChildServices(child.ZoneServiceConfigId, childrenServiceIds);
            }
        }

        private class CacheManager : BaseCacheManager
        {
            IZoneServiceConfigDataManager dataManager = BEDataManagerFactory.GetDataManager<IZoneServiceConfigDataManager>();

            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return dataManager.AreZoneServiceConfigsUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region  Mappers
        private ZoneServiceConfigInfo ZoneServiceConfigInfoMapper(ZoneServiceConfig zoneServiceConfig)
        {
            return new ZoneServiceConfigInfo()
            {
                ZoneServiceConfigId = zoneServiceConfig.ZoneServiceConfigId,
                Symbol = zoneServiceConfig.Symbol
            };
        }

        private ZoneServiceConfigDetail ZoneServiceConfigDetailMapper(ZoneServiceConfig zoneServiceConfig)
        {
            ZoneServiceConfigDetail zoneServiceConfigDetail = new ZoneServiceConfigDetail()
            {
                Entity = zoneServiceConfig
            };
            if (zoneServiceConfig.Settings.ParentId.HasValue)
            {

                var parentzoneServiceConfig = GetZoneServiceConfig(zoneServiceConfig.Settings.ParentId.Value);
                if (parentzoneServiceConfig == null)
                    throw new DataIntegrityValidationException(String.Format("Parent Zone Service  '{0}' does not exist", (zoneServiceConfig.Settings.ParentId)));

                zoneServiceConfigDetail.ParentName = parentzoneServiceConfig != null ? parentzoneServiceConfig.Settings.Name : null;
            }
            return zoneServiceConfigDetail;

        }
        #endregion

    }
}