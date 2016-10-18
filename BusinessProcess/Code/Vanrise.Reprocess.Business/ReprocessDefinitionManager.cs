using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Reprocess.Data;
using Vanrise.Reprocess.Entities;
using Vanrise.Entities;
using Vanrise.Common;


namespace Vanrise.Reprocess.Business
{
    public class ReprocessDefinitionManager
    {
        #region Public Methods

        public IDataRetrievalResult<ReprocessDefinitionDetail> GetFilteredReprocessDefinitions(DataRetrievalInput<ReprocessDefinitionQuery> input)
        {
            var allReprocessDefinitions = this.GetCachedReprocessDefinitions();
            Func<ReprocessDefinition, bool> filterExpression = (x) => (input.Query.Name == null || x.Name.ToLower().Contains(input.Query.Name.ToLower()));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allReprocessDefinitions.ToBigResult(input, filterExpression, ReprocessDefinitionDetailMapper));
        }

        public ReprocessDefinition GetReprocessDefinition(Guid reprocessDefinitionId)
        {
            Dictionary<Guid, ReprocessDefinition> cachedReprocessDefinitions = this.GetCachedReprocessDefinitions();
            return cachedReprocessDefinitions.GetRecord(reprocessDefinitionId);
        }

        public Vanrise.Entities.InsertOperationOutput<ReprocessDefinitionDetail> AddReprocessDefinition(ReprocessDefinition reprocessDefinitionItem)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<ReprocessDefinitionDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            reprocessDefinitionItem.ReprocessDefinitionId = Guid.NewGuid();
            IReprocessDefinitionDataManager dataManager = ReprocessDataManagerFactory.GetDataManager<IReprocessDefinitionDataManager>();

            if (dataManager.Insert(reprocessDefinitionItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = ReprocessDefinitionDetailMapper(reprocessDefinitionItem);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<ReprocessDefinitionDetail> UpdateReprocessDefinition(ReprocessDefinition reprocessDefinitionItem)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<ReprocessDefinitionDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IReprocessDefinitionDataManager dataManager = ReprocessDataManagerFactory.GetDataManager<IReprocessDefinitionDataManager>();

            if (dataManager.Update(reprocessDefinitionItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = ReprocessDefinitionDetailMapper(this.GetReprocessDefinition(reprocessDefinitionItem.ReprocessDefinitionId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public IEnumerable<ReprocessDefinitionInfo> GetReprocessDefinitionsInfo(ReprocessDefinitionInfoFilter filter)
        {
            var allReprocessDefinitions = this.GetCachedReprocessDefinitions();

            if (filter != null)
            {
                Func<ReprocessDefinition, bool> filterExpression = (x) =>
                {
                    return true;
                };
                return allReprocessDefinitions.FindAllRecords(filterExpression).MapRecords(ReprocessDefinitionMapper);
            }
            else
            {
                return allReprocessDefinitions.MapRecords(ReprocessDefinitionMapper);
            }
        }

        #endregion


        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IReprocessDefinitionDataManager _dataManager = ReprocessDataManagerFactory.GetDataManager<IReprocessDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreReprocessDefinitionUpdated(ref _updateHandle);
            }
        }

        #endregion


        #region Private Methods

        Dictionary<Guid, ReprocessDefinition> GetCachedReprocessDefinitions()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetReprocessDefinitions",
               () =>
               {
                   IReprocessDefinitionDataManager dataManager = ReprocessDataManagerFactory.GetDataManager<IReprocessDefinitionDataManager>();
                   return dataManager.GetReprocessDefinition().ToDictionary(x => x.ReprocessDefinitionId, x => x);
               });
        }

        #endregion


        #region Mappers

        private ReprocessDefinitionDetail ReprocessDefinitionDetailMapper(ReprocessDefinition reprocessDefinition)
        {
            ReprocessDefinitionDetail reprocessDefinitionDetail = new ReprocessDefinitionDetail()
            {
                Entity = reprocessDefinition
            };
            return reprocessDefinitionDetail;
        }


        private ReprocessDefinitionInfo ReprocessDefinitionMapper(ReprocessDefinition reprocessDefinition)
        {
            if (reprocessDefinition == null)
                return null;

            return new ReprocessDefinitionInfo()
            {
                ReprocessDefinitionId = reprocessDefinition.ReprocessDefinitionId,
                Name = reprocessDefinition.Name
            };
        }
        #endregion
    }
}
