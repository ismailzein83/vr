using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace Vanrise.BusinessProcess.Business
{
    public class BPDefinitionManager
    {
        public Vanrise.Entities.IDataRetrievalResult<BPDefinitionDetail> GetFilteredBPDefinitions(Vanrise.Entities.DataRetrievalInput<BPDefinitionQuery> input)
        {
            var allBPDefinitions = GetCachedBPDefinitions();

            Func<BPDefinition, bool> filterExpression = (prod) =>
                 (input.Query.Title == null || prod.Title.ToLower().Contains(input.Query.Title.ToLower()));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allBPDefinitions.ToBigResult(input, filterExpression, BPDefinitionDetailMapper));
        }

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

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IBPDefinitionDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return dataManager.AreBPDefinitionsUpdated(ref _updateHandle);
            }
        }
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
    }
}