using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.Common.Business
{
    public class StatusDefinitionManager : IBusinessEntityManager
    {

        #region Public Methods
        public StatusDefinition GetStatusDefinition(Guid statusDefinitionId)
        {
            Dictionary<Guid, StatusDefinition> cachedStatusDefinitions = this.GetCachedStatusDefinitions();
            return cachedStatusDefinitions.GetRecord(statusDefinitionId);
        }
        public string GetStatusDefinitionName(Guid statusDefinitionId)
        {
            StatusDefinition statusDefinition = this.GetStatusDefinition(statusDefinitionId);
            return (statusDefinition != null) ? statusDefinition.Name : null;
        }

        public IDataRetrievalResult<StatusDefinitionDetail> GetFilteredStatusDefinitions(DataRetrievalInput<StatusDefinitionQuery> input)
        {
            var allStatusDefinitions = GetCachedStatusDefinitions();
            Func<StatusDefinition, bool> filterExpression = (x) =>
            {
                if (input.Query.BusinessEntityDefinitionId.HasValue && input.Query.BusinessEntityDefinitionId != x.BusinessEntityDefinitionId)
                    return false;
                if (input.Query.Name != null && !x.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                return true;
            };
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allStatusDefinitions.ToBigResult(input, filterExpression, StatusDefinitionDetailMapper));
        }

        public Vanrise.Entities.InsertOperationOutput<StatusDefinitionDetail> AddStatusDefinition(StatusDefinition statusDefinitionItem)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<StatusDefinitionDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IStatusDefinitionDataManager dataManager = CommonDataManagerFactory.GetDataManager<IStatusDefinitionDataManager>();

            statusDefinitionItem.StatusDefinitionId = Guid.NewGuid();

            if (dataManager.Insert(statusDefinitionItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = StatusDefinitionDetailMapper(statusDefinitionItem);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<StatusDefinitionDetail> UpdateStatusDefinition(StatusDefinition statusDefinitionItem)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<StatusDefinitionDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IStatusDefinitionDataManager dataManager = CommonDataManagerFactory.GetDataManager<IStatusDefinitionDataManager>();

            if (dataManager.Update(statusDefinitionItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = StatusDefinitionDetailMapper(this.GetStatusDefinition(statusDefinitionItem.StatusDefinitionId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public IEnumerable<StatusDefinitionInfo> GetStatusDefinitionsInfo(StatusDefinitionFilter filter)
        {
            Func<StatusDefinition, bool> filterExpression = null;

            if (filter != null)
            {
                filterExpression = (x) =>
                {
                    if (filter.BusinessEntityDefinitionId.HasValue && filter.BusinessEntityDefinitionId.Value != x.BusinessEntityDefinitionId)
                        return false;
                    return true;
                };
            }
            return this.GetCachedStatusDefinitions().MapRecords(StatusDefinitionInfoMapper, filterExpression).OrderBy(x => x.Name);
        }

        public IEnumerable<StatusDefinition> GetFilteredStatusDefinitions(StatusDefinitionFilter filter)
        {
            Func<StatusDefinition, bool> filterExpression = null;

            if (filter != null)
            {
                filterExpression = (x) =>
                {
                    if (filter.BusinessEntityDefinitionId.HasValue && filter.BusinessEntityDefinitionId.Value != x.BusinessEntityDefinitionId)
                        return false;
                    return true;
                };
            }
              
            return GetCachedStatusDefinitions().FindAllRecords(filterExpression);
        }

        public IEnumerable<StatusDefinition> GetAllStatusDefinitions()
        {
            return this.GetCachedStatusDefinitions().MapRecords(x => x).OrderBy(x => x.Name);
        }

        #endregion

        #region IBusinessEntityManager
        public string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetStatusDefinitionName(Guid.Parse(context.EntityId.ToString()));
        }
        public dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetStatusDefinition(context.EntityId);
        }
        public dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }
        public List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            return GetAllStatusDefinitions().Select(itm => itm as dynamic).ToList();
        }
        public bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            throw new NotImplementedException();
        }
        public dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }
      
        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IStatusDefinitionDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IStatusDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreStatusDefinitionUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Private Methods
        Dictionary<Guid, StatusDefinition> GetCachedStatusDefinitions()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetStatusDefinition",
               () =>
               {
                   IStatusDefinitionDataManager dataManager = CommonDataManagerFactory.GetDataManager<IStatusDefinitionDataManager>();
                   return dataManager.GetStatusDefinition().ToDictionary(x => x.StatusDefinitionId, x => x);
               });
        }

        #endregion

        #region Mappers

        public StatusDefinitionDetail StatusDefinitionDetailMapper(StatusDefinition statusDefinition)
        {
            IBusinessEntityDefinitionManager manager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IBusinessEntityDefinitionManager>();
            StatusDefinitionDetail satatusDefinitionDetail = new StatusDefinitionDetail()
            {
                Entity = statusDefinition,
                StatusGroupDescription = manager.GetBusinessEntityDefinitionName(statusDefinition.BusinessEntityDefinitionId)
            };
            return satatusDefinitionDetail;
        }

        public StatusDefinitionInfo StatusDefinitionInfoMapper(StatusDefinition statusDefinition)
        {
            StatusDefinitionInfo statusDefinitionInfo = new StatusDefinitionInfo()
            {
                StatusDefinitionId = statusDefinition.StatusDefinitionId,
                Name = statusDefinition.Name
            };
            return statusDefinitionInfo;
        }
        #endregion
    }
}
