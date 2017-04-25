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
    public class GenericLKUPManager : IBusinessEntityManager
    {

       



        //VRNotificationTypeManager _vrnotificationTypeManager = new VRNotificationTypeManager();
        public GenericLKUPItem GetGenericLKUPItem(Guid genericLKUPItemId)
        {
            Dictionary<Guid, GenericLKUPItem> cachedGenericLKUPItems = this.GetCachedGenericLKUPItems();

            return cachedGenericLKUPItems.GetRecord(genericLKUPItemId);
        }

        public string GetGenericLKUPItemName(Guid genericLKUPItemId)
        {
            GenericLKUPItem genericLKUPItem = this.GetGenericLKUPItem(genericLKUPItemId);
            return (genericLKUPItem != null) ? genericLKUPItem.Name : null;
        }

        public IDataRetrievalResult<GenericLKUPItemDetail> GetFilteredGenericLKUPItems(DataRetrievalInput<GenericLKUPQuery> input)
        {
            var allGenericLKUPItems = GetCachedGenericLKUPItems();
            Func<GenericLKUPItem, bool> filterExpression = (x) =>
            {
                if (input.Query.BusinessEntityDefinitionIds != null && !input.Query.BusinessEntityDefinitionIds.Contains(x.BusinessEntityDefinitionId))
                    return false;
                if (input.Query.Name != null && !x.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                return true;
            };
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult<GenericLKUPItemDetail>(input, allGenericLKUPItems.ToBigResult(input, filterExpression, GenericLKUPItemDetailMapper));
        }
        public Vanrise.Entities.InsertOperationOutput<GenericLKUPItemDetail> AddGenericLKUPItem(GenericLKUPItem genericLKUPItem)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<GenericLKUPItemDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IGenericLKUPItemDataManager dataManager = CommonDataManagerFactory.GetDataManager<IGenericLKUPItemDataManager>();

            genericLKUPItem.GenericLKUPItemId = Guid.NewGuid();

            if (dataManager.Insert(genericLKUPItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = GenericLKUPItemDetailMapper(genericLKUPItem);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<GenericLKUPItemDetail> UpdateGenericLKUPItem(GenericLKUPItem genericLKUPItem)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<GenericLKUPItemDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IGenericLKUPItemDataManager dataManager = CommonDataManagerFactory.GetDataManager<IGenericLKUPItemDataManager>();

            if (dataManager.Update(genericLKUPItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = GenericLKUPItemDetailMapper(this.GetGenericLKUPItem(genericLKUPItem.GenericLKUPItemId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }
        public IEnumerable<GenericLKUPItem> GetAllGenericLKUPItems()
        {
            return this.GetCachedGenericLKUPItems().MapRecords(x => x).OrderBy(x => x.Name);
        }
       
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IGenericLKUPItemDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IGenericLKUPItemDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreGenericLKUPItemUpdated(ref _updateHandle);
            }
        }



        Dictionary<Guid, GenericLKUPItem> GetCachedGenericLKUPItems()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedGenericLKUPItems",
               () =>
               {
                   IGenericLKUPItemDataManager dataManager = CommonDataManagerFactory.GetDataManager<IGenericLKUPItemDataManager>();
                   return dataManager.GetGenericLKUPItem().ToDictionary(x => x.GenericLKUPItemId, x => x);
               });
        }
        private GenericLKUPItemDetail GenericLKUPItemDetailMapper(GenericLKUPItem genericLKUPItem)
        {
            IBusinessEntityDefinitionManager manager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IBusinessEntityDefinitionManager>();
            GenericLKUPItemDetail genericLKUPItemDetail = new GenericLKUPItemDetail()
            {
                Entity = genericLKUPItem,
                BusinessEntityDefinitionName = manager.GetBusinessEntityDefinitionName(genericLKUPItem.BusinessEntityDefinitionId)
            };
            return genericLKUPItemDetail;
        }
       
        #region IBusinessEntityManager

        public string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetGenericLKUPItemName(Guid.Parse(context.EntityId.ToString()));
        }

        public dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            var genericLKUPItem = context.Entity as GenericLKUPItem;
            return genericLKUPItem.GenericLKUPItemId;
        }

        public dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetGenericLKUPItem(context.EntityId);
        }

        public dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }

        public List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            return GetAllGenericLKUPItems().Select(itm => itm as dynamic).ToList();
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
    }
}


