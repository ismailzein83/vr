using Demo.Module.Data;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Demo.Module.Business
{
    public class ItemManager
    {
        ProductManager _productManager = new ProductManager();

        #region Public Methods
        public IDataRetrievalResult<ItemDetails> GetFilteredItems(DataRetrievalInput<ItemQuery> input)
        {
            var allItems = GetCachedItems();
            Func<Item, bool> filterExpression = (item) =>
            {
                if (input.Query.Name != null && !item.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                if (input.Query.ProductIds != null && !input.Query.ProductIds.Contains(item.ProductId))
                    return false;
                return true;
            };
            return DataRetrievalManager.Instance.ProcessResult(input, allItems.ToBigResult(input, filterExpression, ItemDetailMapper));

        }

        public IEnumerable<ItemShapeConfig> GetItemShapeConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<ItemShapeConfig>(ItemShapeConfig.EXTENSION_TYPE);
        }

        public InsertOperationOutput<ItemDetails> AddItem(Item item)
        {
            IItemDataManager itemDataManager = DemoModuleFactory.GetDataManager<IItemDataManager>();
            InsertOperationOutput<ItemDetails> insertOperationOutput = new InsertOperationOutput<ItemDetails>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            long itemId = -1;

            bool insertActionSuccess = itemDataManager.Insert(item, out itemId);
            if (insertActionSuccess)
            {
                item.ItemId = itemId;
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = ItemDetailMapper(item);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
        }
        public Item GetItemById(long itemId)
        {
            var allItems = GetCachedItems();
            return allItems.GetRecord(itemId);
        }

        public UpdateOperationOutput<ItemDetails> UpdateItem(Item item)
        {
            IItemDataManager itemDataManager = DemoModuleFactory.GetDataManager<IItemDataManager>();
            UpdateOperationOutput<ItemDetails> updateOperationOutput = new UpdateOperationOutput<ItemDetails>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            bool updateActionSuccess = itemDataManager.Update(item);
            if (updateActionSuccess)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = ItemDetailMapper(item);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }
        #endregion

        #region Private Classes
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IItemDataManager itemDataManager = DemoModuleFactory.GetDataManager<IItemDataManager>();
            object _updateHandle;
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return itemDataManager.AreItemsUpdated(ref _updateHandle);
            }
        }
        #endregion

        #region Private Methods

        private Dictionary<long, Item> GetCachedItems()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
               .GetOrCreateObject("GetCachedItems", () =>
               {
                   IItemDataManager itemDataManager = DemoModuleFactory.GetDataManager<IItemDataManager>();
                   List<Item> items = itemDataManager.GetItems();
                   return items.ToDictionary(item => item.ItemId, item => item);
               });
        }
        #endregion

        #region Mappers
        public ItemDetails ItemDetailMapper(Item item)
        {
            var itemDetails = new ItemDetails
            {
                Name = item.Name,
                ItemId = item.ItemId,
                ProductName = _productManager.GetProductName(item.ProductId)
            };

            if (item.Settings != null && item.Settings.ItemShape != null)
            {
                var context = new ItemShapeDescriptionContext
                {
                    Item = item
                };
                itemDetails.AreaDescription = item.Settings.ItemShape.GetItemAreaDescription(context);
            }

            return itemDetails;

        }
        #endregion
    }
}
