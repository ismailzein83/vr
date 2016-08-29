using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BEBridge.Data;
using Vanrise.BEBridge.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.BEBridge.Business
{
    public class BEReceiveDefinitionManager
    {
        #region Public Methods
        public BEReceiveDefinition GetBEReceiveDefinition(Guid id)
        {
            var allBEReceiveDefinitions = GetCachedBEReceiveDefinitions();
            return allBEReceiveDefinitions.GetRecord(id);
        }

        public IEnumerable<BEReceiveDefinitionInfo> GetBEReceiveDefinitionsInfo()
        {
            return GetCachedBEReceiveDefinitions().MapRecords(BEReceiveDefinitionInfoMapper);
        }
        public IDataRetrievalResult<BEReceiveDefinitionDetail> GetFilteredBeReceiveDefinitions(DataRetrievalInput<BEReceiveDefinitionQuery> input)
        {
            var receiveDefinitions = GetCachedBEReceiveDefinitions().Values.ToList();
            Func<BEReceiveDefinition, bool> filterExpression = x => ((input.Query.Name == null || x.Name.ToLower().Contains(input.Query.Name.ToLower())));
            return DataRetrievalManager.Instance.ProcessResult(input, receiveDefinitions.ToBigResult(input, filterExpression, BeReceiveDefinitionDetailMapper));
        }
        public BEReceiveDefinition GetReceiveDefinition(Guid receiveDefinitionId)
        {
            return GetCachedBEReceiveDefinitions().GetRecord(receiveDefinitionId);
        }
        #endregion

        #region Private Methods
        Dictionary<Guid, BEReceiveDefinition> GetCachedBEReceiveDefinitions()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetBEReceiveDefinitions",
               () =>
               {
                   IBEReceiveDefinitionDataManager dataManager = BEBridgeDataManagerFactory.GetDataManager<IBEReceiveDefinitionDataManager>();
                   IEnumerable<BEReceiveDefinition> carrierAccounts = dataManager.GetBEReceiveDefinitions();
                   return carrierAccounts.ToDictionary(kvp => kvp.BEReceiveDefinitionId, kvp => kvp);
               });
        }

        BEReceiveDefinitionInfo BEReceiveDefinitionInfoMapper(BEReceiveDefinition beDefinition)
        {
            return new BEReceiveDefinitionInfo
            {
                Id = beDefinition.BEReceiveDefinitionId,
                Name = beDefinition.Name
            };
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IBEReceiveDefinitionDataManager _dataManager = BEBridgeDataManagerFactory.GetDataManager<IBEReceiveDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreBEReceiveDefinitionsUpdated(ref _updateHandle);
            }
        }

        #endregion
        #region DetailMappers

        public BEReceiveDefinitionDetail BeReceiveDefinitionDetailMapper(BEReceiveDefinition beReceiveDefinition)
        {
            BEReceiveDefinitionDetail beReceiveDefinitionDetail = new BEReceiveDefinitionDetail
            {
                Entity = beReceiveDefinition,
                Description = ""
            };
            return beReceiveDefinitionDetail;
        }
        #endregion
    }
}
