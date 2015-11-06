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
        public IDataRetrievalResult<ZoneServiceConfig> GetFilteredZoneServiceConfigs(DataRetrievalInput<ZoneServiceConfigQuery> input)
        {
            var allZoneServiceConfigs = GetCachedZoneServiceConfigs();
            Func<ZoneServiceConfig, bool> filterExpression = (x) => (input.Query.Name == null || x.Name.ToLower().Contains(input.Query.Name.ToLower()));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allZoneServiceConfigs.ToBigResult(input, filterExpression));

        }

        public Dictionary<Int16, ZoneServiceConfig> GetCachedZoneServiceConfigs()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetZoneServiceConfigs",
               () =>
               {
                   IZoneServiceConfigDataManager dataManager = BEDataManagerFactory.GetDataManager<IZoneServiceConfigDataManager>();
                   IEnumerable<ZoneServiceConfig> rateTypes = dataManager.GetZoneServiceConfigs();
                   return rateTypes.ToDictionary(x => x.ServiceFlag, x => x);
               });
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


        public IEnumerable<ZoneServiceConfig> GetAllZoneServiceConfigs()
        {
            var allZoneServiceConfigs = GetCachedZoneServiceConfigs();
            if (allZoneServiceConfigs == null)
                return null;
            return allZoneServiceConfigs.Values;
        }

        public ZoneServiceConfig GetZoneServiceConfig(Int16 serviceFlag)
        {
            var allZoneServiceConfigs = GetCachedZoneServiceConfigs();
            return allZoneServiceConfigs.GetRecord(serviceFlag);
        }

        public TOne.Entities.InsertOperationOutput<ZoneServiceConfig> AddZoneServiceConfig(ZoneServiceConfig zoneServiceConfig)
        {
            TOne.Entities.InsertOperationOutput<ZoneServiceConfig> insertOperationOutput = new TOne.Entities.InsertOperationOutput<ZoneServiceConfig>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IZoneServiceConfigDataManager dataManager = BEDataManagerFactory.GetDataManager<IZoneServiceConfigDataManager>();
            bool insertActionSucc = dataManager.Insert(zoneServiceConfig);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = zoneServiceConfig;
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        public TOne.Entities.UpdateOperationOutput<ZoneServiceConfig> UpdateZoneServiceConfig(ZoneServiceConfig zoneServiceConfig)
        {
            IZoneServiceConfigDataManager dataManager = BEDataManagerFactory.GetDataManager<IZoneServiceConfigDataManager>();

            bool updateActionSucc = dataManager.Update(zoneServiceConfig);
            TOne.Entities.UpdateOperationOutput<ZoneServiceConfig> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<ZoneServiceConfig>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = zoneServiceConfig;
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

    }
}
