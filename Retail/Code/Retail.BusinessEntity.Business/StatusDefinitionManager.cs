using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace Retail.BusinessEntity.Business
{
    public class StatusDefinitionManager : BaseBusinessEntityManager
    {
        #region Public Methods

        public Retail.BusinessEntity.Entities.StatusDefinition GetStatusDefinition(Guid statusDefinitionId)
        {
            Dictionary<Guid, Retail.BusinessEntity.Entities.StatusDefinition> cachedStatusDefinitions = this.GetCachedStatusDefinitions();
            return cachedStatusDefinitions.GetRecord(statusDefinitionId);
        }

        public string GetStatusDefinitionName(Guid statusDefinitionId)
        {
            Retail.BusinessEntity.Entities.StatusDefinition statusDefinition = this.GetStatusDefinition(statusDefinitionId);
            return (statusDefinition != null) ? statusDefinition.Name : null;
        }

        public IDataRetrievalResult<Retail.BusinessEntity.Entities.StatusDefinitionDetail> GetFilteredStatusDefinitions(DataRetrievalInput<Retail.BusinessEntity.Entities.StatusDefinitionQuery> input)
        {
            var allStatusDefinitions = GetCachedStatusDefinitions();
            Func<Retail.BusinessEntity.Entities.StatusDefinition, bool> filterExpression = (x) => ((input.Query.Name == null || x.Name.ToLower().Contains(input.Query.Name.ToLower())) &&
                                                                    (input.Query.EntityTypes == null || input.Query.EntityTypes.Contains(x.EntityType)));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allStatusDefinitions.ToBigResult(input, filterExpression, StatusDefinitionDetailMapper));
        }

        public Vanrise.Entities.InsertOperationOutput<Retail.BusinessEntity.Entities.StatusDefinitionDetail> AddStatusDefinition(Retail.BusinessEntity.Entities.StatusDefinition statusDefinitionItem)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<Retail.BusinessEntity.Entities.StatusDefinitionDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IStatusDefinitionDataManager dataManager = BEDataManagerFactory.GetDataManager<IStatusDefinitionDataManager>();

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

        public Vanrise.Entities.UpdateOperationOutput<Retail.BusinessEntity.Entities.StatusDefinitionDetail> UpdateStatusDefinition(Retail.BusinessEntity.Entities.StatusDefinition statusDefinitionItem)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<Retail.BusinessEntity.Entities.StatusDefinitionDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IStatusDefinitionDataManager dataManager = BEDataManagerFactory.GetDataManager<IStatusDefinitionDataManager>();

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

        public IEnumerable<Retail.BusinessEntity.Entities.StatusDefinitionInfo> GetStatusDefinitionsInfo(Retail.BusinessEntity.Entities.StatusDefinitionFilter filter)
        {
            Func<Retail.BusinessEntity.Entities.StatusDefinition, bool> filterExpression = null;
            if (filter != null)
            {
                filterExpression = (item) =>
                {
                    if (filter.EntityType == null || item.EntityType == filter.EntityType)
                        return true;
                    return false;
                };
            }
            return this.GetCachedStatusDefinitions().MapRecords(StatusDefinitionInfoMapper, filterExpression).OrderBy(x => x.Name);
        }

        public IEnumerable<Retail.BusinessEntity.Entities.StatusDefinition> GetFilteredStatusDefinitions(Retail.BusinessEntity.Entities.StatusDefinitionFilter filter)
        {
            Func<Retail.BusinessEntity.Entities.StatusDefinition, bool> filterExpression = null;
            if (filter != null)
            {
                filterExpression = item =>
                {
                    if (filter.EntityType == null || item.EntityType == filter.EntityType)
                        return true;
                    return false;
                };
            }
            return GetCachedStatusDefinitions().FindAllRecords(filterExpression);
        }

        public IEnumerable<Retail.BusinessEntity.Entities.StatusDefinition> GetAllStatusDefinitions()
        {
            return this.GetCachedStatusDefinitions().MapRecords(x => x).OrderBy(x => x.Name);
        }
             
        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IStatusDefinitionDataManager _dataManager = BEDataManagerFactory.GetDataManager<IStatusDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreStatusDefinitionUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Private Methods

        Dictionary<Guid, Retail.BusinessEntity.Entities.StatusDefinition> GetCachedStatusDefinitions()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetStatusDefinition",
               () =>
               {
                   IStatusDefinitionDataManager dataManager = BEDataManagerFactory.GetDataManager<IStatusDefinitionDataManager>();
                   return dataManager.GetStatusDefinition().ToDictionary(x => x.StatusDefinitionId, x => x);
               });
        }

        #endregion

        #region Mappers

        public Retail.BusinessEntity.Entities.StatusDefinitionDetail StatusDefinitionDetailMapper(Retail.BusinessEntity.Entities.StatusDefinition statusDefinition)
        {
            Retail.BusinessEntity.Entities.StatusDefinitionDetail satatusDefinitionDetail = new Retail.BusinessEntity.Entities.StatusDefinitionDetail()
            {
                Entity = statusDefinition,
                EntityTypeDescription = Utilities.GetEnumDescription(statusDefinition.EntityType)
            };
            return satatusDefinitionDetail;
        }

        public Retail.BusinessEntity.Entities.StatusDefinitionInfo StatusDefinitionInfoMapper(Retail.BusinessEntity.Entities.StatusDefinition statusDefinition)
        {
            Retail.BusinessEntity.Entities.StatusDefinitionInfo statusDefinitionInfo = new Retail.BusinessEntity.Entities.StatusDefinitionInfo()
            {
                StatusDefinitionId = statusDefinition.StatusDefinitionId,
                Name = statusDefinition.Name
            };
            return statusDefinitionInfo;
        }
        #endregion

        #region IBusinessEntityManager

        public override List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            return GetAllStatusDefinitions().Select(itm => itm as dynamic).ToList();
        }

        public override dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetStatusDefinition(context.EntityId);
        }

        public override string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetStatusDefinitionName(Guid.Parse(context.EntityId.ToString()));
        }

        public override dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            var statusDefinition = context.Entity as Retail.BusinessEntity.Entities.StatusDefinition;
            return statusDefinition.StatusDefinitionId;
        }

        public override IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public override dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public override bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            throw new NotImplementedException();
        }

        public override dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
