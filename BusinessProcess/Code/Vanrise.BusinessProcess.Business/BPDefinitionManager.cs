using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace Vanrise.BusinessProcess.Business
{
    public class BPDefinitionManager
    {
        #region public methods

        public IEnumerable<BPDefinition> GetBPDefinitions()
        {
            var cachedDefinitions = GetCachedBPDefinitions();
            if (cachedDefinitions != null)
                return cachedDefinitions.Values;
            else
                return null;
        }

        public Vanrise.Entities.IDataRetrievalResult<BPDefinitionDetail> GetFilteredBPDefinitions(Vanrise.Entities.DataRetrievalInput<BPDefinitionQuery> input)
        {
            var allBPDefinitions = GetCachedBPDefinitions();

            Func<BPDefinition, bool> filterExpression = (prod) =>
            {
                if (prod.Configuration.IsSubProcess)
                    return false;

                if (!string.IsNullOrEmpty(input.Query.Title) && !prod.Title.ToLower().Contains(input.Query.Title.ToLower()))
                    return false;

                return true;
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allBPDefinitions.ToBigResult(input, filterExpression, BPDefinitionDetailMapper));
        }

        public IEnumerable<BPDefinitionInfo> GetBPDefinitionsInfo(BPDefinitionInfoFilter filter)
        {
            var bpDefinitions = GetCachedBPDefinitions();

            if (filter != null)
            {
                Func<BPDefinition, bool> filterExpression = (x) => (true);
                return bpDefinitions.FindAllRecords(filterExpression).MapRecords(BPDefinitionInfoMapper);
            }
            else
            {
                return bpDefinitions.MapRecords(BPDefinitionInfoMapper);
            }
        }

        public BPDefinition GetBPDefinition(int definitionId)
        {
            return GetCachedBPDefinitions().GetRecord(definitionId);
        }
        public BPDefinition GetDefinition(string processName)
        {
            return GetBPDefinitions().FirstOrDefault(itm => itm.Name == processName);
        }

        #endregion

        #region private methods
        private Dictionary<int, BPDefinition> GetCachedBPDefinitions()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetBPDefinitions",
               () =>
               {
                   IBPDefinitionDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPDefinitionDataManager>();
                   IEnumerable<BPDefinition> accounts = dataManager.GetBPDefinitions();
                   return accounts.ToDictionary(cn => cn.BPDefinitionID, cn => cn);
               });
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IBPDefinitionDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return dataManager.AreBPDefinitionsUpdated(ref _updateHandle);
            }
        }
        #endregion

        #region mapper
        private BPDefinitionDetail BPDefinitionDetailMapper(BPDefinition bpDefinition)
        {
            if (bpDefinition == null)
                return null;

            return new BPDefinitionDetail()
            {
                Entity = bpDefinition,
            };
        }

        private BPDefinitionInfo BPDefinitionInfoMapper(BPDefinition bpDefinition)
        {
            if (bpDefinition == null)
                return null;

            return new BPDefinitionInfo()
            {
                BPDefinitionID = bpDefinition.BPDefinitionID,
                Name = bpDefinition.Title
            };
        }
        #endregion
    }
}