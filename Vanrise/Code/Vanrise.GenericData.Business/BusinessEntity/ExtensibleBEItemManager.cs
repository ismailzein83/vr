using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Data;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.Caching;
using Vanrise.Entities;
namespace Vanrise.GenericData.Business
{
    public class ExtensibleBEItemManager
    {
     
        #region Public Methods
        private DataRecordTypeManager _dataRecordTypeManager;
        public ExtensibleBEItemManager()
        {
            _dataRecordTypeManager = new DataRecordTypeManager();
        }
        public ExtensibleBEItem GetExtensibleBEItem(int extensibleBEItemId)
        {
            var cachedExtensibleBEItems = GetCachedExtensibleBEItems();
            return cachedExtensibleBEItems.GetRecord(extensibleBEItemId);

        }
        public Vanrise.Entities.IDataRetrievalResult<ExtensibleBEItemDetail> GetFilteredExtensibleBEItems(Vanrise.Entities.DataRetrievalInput<ExtensibleBEItemQuery> input)
        {
            var cachedExtensibleBEItems = GetCachedExtensibleBEItems();

            Func<ExtensibleBEItem, bool> filterExpression = (extensibleBEItem) => (input.Query.BusinessEntityDefinitionId == extensibleBEItem.BusinessEntityDefinitionId);
            return DataRetrievalManager.Instance.ProcessResult(input, cachedExtensibleBEItems.ToBigResult(input, filterExpression, ExtensibleBEItemDetailMapper));
        }
        public Vanrise.Entities.UpdateOperationOutput<ExtensibleBEItemDetail> UpdateExtensibleBEItem(ExtensibleBEItem extensibleBEItem)
        {
            UpdateOperationOutput<ExtensibleBEItemDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<ExtensibleBEItemDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IExtensibleBEItemDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IExtensibleBEItemDataManager>();
            bool updateActionSucc = dataManager.UpdateExtensibleBEItem(extensibleBEItem);

            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.UpdatedObject = ExtensibleBEItemDetailMapper(extensibleBEItem);
            }
            return updateOperationOutput;
        }
        public Vanrise.Entities.InsertOperationOutput<ExtensibleBEItemDetail> AddExtensibleBEItem(ExtensibleBEItem extensibleBEItem)
        {
            InsertOperationOutput<ExtensibleBEItemDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<ExtensibleBEItemDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int extensibleBEItemId = -1;
            var cachedExtensibleBEItems = GetCachedExtensibleBEItems();
            if (!cachedExtensibleBEItems.Any(x => x.Value.BusinessEntityDefinitionId == extensibleBEItem.BusinessEntityDefinitionId && x.Value.DataRecordTypeId == extensibleBEItem.DataRecordTypeId))
            {
                IExtensibleBEItemDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IExtensibleBEItemDataManager>();
                bool insertActionSucc = dataManager.AddExtensibleBEItem(extensibleBEItem, out extensibleBEItemId);

                if (insertActionSucc)
                {
                    insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                    extensibleBEItem.ExtensibleBEItemId = extensibleBEItemId;
                    insertOperationOutput.InsertedObject = ExtensibleBEItemDetailMapper(extensibleBEItem);

                    CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                }
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;

            }

            return insertOperationOutput;
        }
        public ExtensibleBEItem GetExtensibleBEItem(int businessEntityId, Guid dataRecordTypeId)
        {
            var cachedExtensibleBEItems = GetCachedExtensibleBEItems();
            var extensibleBEItem = cachedExtensibleBEItems.FindRecord(x => x.BusinessEntityDefinitionId == businessEntityId && x.DataRecordTypeId == dataRecordTypeId);
            if (extensibleBEItem == null)
                throw new NullReferenceException(string.Format("ExtensibleBEItem for business entity {0} of data record type id {1}", businessEntityId, dataRecordTypeId));
            return extensibleBEItem; ;
        }
        public IEnumerable<ExtensibleBEItem> GetAllExtensibleBEItems()
        {
            var cachedExtensibleBEItems = GetCachedExtensibleBEItems();
            return cachedExtensibleBEItems.Values;
        }
        #endregion
     
        #region Private Methods

        private Dictionary<int, ExtensibleBEItem> GetCachedExtensibleBEItems()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetExtensibleBEItems",
               () =>
               {
                   IExtensibleBEItemDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IExtensibleBEItemDataManager>();
                   IEnumerable<ExtensibleBEItem> extensibleBEItems = dataManager.GetExtensibleBEItems();
                   return extensibleBEItems.ToDictionary(kvp => kvp.ExtensibleBEItemId, kvp => kvp);
               });
        }
        #endregion

        #region Private Classes
        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IExtensibleBEItemDataManager _dataManager = GenericDataDataManagerFactory.GetDataManager<IExtensibleBEItemDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired()
            {
                return _dataManager.AreExtensibleBEItemUpdated(ref _updateHandle);
            }
        }
        #endregion

        #region Mappers

        private ExtensibleBEItemDetail ExtensibleBEItemDetailMapper(ExtensibleBEItem extensibleBEItem)
        {
            ExtensibleBEItemDetail extensibleBEItemDetail = new ExtensibleBEItemDetail();
            extensibleBEItemDetail.Entity = extensibleBEItem;
            extensibleBEItemDetail.RecordTypeName = _dataRecordTypeManager.GetDataRecordTypeName(extensibleBEItem.DataRecordTypeId);
            return extensibleBEItemDetail;
        }

        #endregion
    }
}
