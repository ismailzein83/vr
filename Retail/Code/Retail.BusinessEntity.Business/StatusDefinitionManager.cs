using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;


namespace Retail.BusinessEntity.Business
{
    public class StatusDefinitionManager
    {
        public StatusDefinition GetStatusDefinition(Guid statusDefinitionId)
        {
            throw new NotImplementedException();
        }

        public IDataRetrievalResult<StatusDefinitionDetail> GetFilteredStatusDefinition(DataRetrievalInput<StatusDefinitionDetail> input)
        {
            var allStatusDefinitions = GetCachedStatusDefinitions();
            //Func<StatusDefinition, bool> filterExpression = (x) => (input.Query.Name == null || x.Name.ToLower().Contains(input.Query.Name.ToLower()));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allStatusDefinitions.ToBigResult(input, null, StatusDefinitionDetailMapper));
        }


        Dictionary<Guid, StatusDefinition> GetCachedStatusDefinitions()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetStatusDefinition",
               () =>
               {
                   IStatusDefinitionDataManager dataManager = BEDataManagerFactory.GetDataManager<IStatusDefinitionDataManager>();
                   return dataManager.GetStatusDefinition().ToDictionary(x => x.StatusDefinitionId, x => x);
               });
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IStatusDefinitionDataManager _dataManager = BEDataManagerFactory.GetDataManager<IStatusDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreStatusDefinitionUpdated(ref _updateHandle);
            }
        }

        public StatusDefinitionDetail StatusDefinitionDetailMapper(StatusDefinition statusDefinition)
        {
            StatusDefinitionDetail satatusDefinitionDetail = new StatusDefinitionDetail()
            {
                Entity = statusDefinition
            };
            return satatusDefinitionDetail;
        }
    }
}
