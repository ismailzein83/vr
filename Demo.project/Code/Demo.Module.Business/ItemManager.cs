using Demo.Module.Data;
using Demo.Module.Entities.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;

namespace Demo.Module.Business
{
    public class ItemManager
    {

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


        public Item GetItemById(int itemId)
        {
            var allItems = GetCachedItems();
            return allItems.GetRecord(itemId);
        }

        private Dictionary<int, Item> GetCachedItems()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
                .GetOrCreateObject("GetCachedItems", () =>
                {
                    IItemDataManager itemDataManager = DemoModuleFactory.GetDataManager<IItemDataManager>();
                    List<Item> items = itemDataManager.GetItems();
                    return items.ToDictionary(item => item.ItemId, item => item);
                });
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IItemDataManager itemDataManager = DemoModuleFactory.GetDataManager<IItemDataManager>();
            object _updateHandle;
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return itemDataManager.AreItemsUpdated(ref _updateHandle);
            }
        }


        public InsertOperationOutput<ItemDetails> AddItem(Item Item)
        {
            IItemDataManager ItemDataManager = DemoModuleFactory.GetDataManager<IItemDataManager>();
            InsertOperationOutput<ItemDetails> insertOperationOutput = new InsertOperationOutput<ItemDetails>();
            int ItemId = -1;
            bool insertActionSuccess = ItemDataManager.Insert(Item, out ItemId);
            if (insertActionSuccess)
            {
                Item.ItemId = ItemId;
                //insertOperationOutput.Result = InsertOperationResult.Succeeded;
               // insertOperationOutput.InsertedObject = ItemDetailMapper(Item);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;

            }
            return insertOperationOutput;

        }

        public UpdateOperationOutput<ItemDetails> UpdateItem(Item Item)
        {
            IItemDataManager ItemDataManager = DemoModuleFactory.GetDataManager<IItemDataManager>();
            UpdateOperationOutput<ItemDetails> updateOperationOutput = new UpdateOperationOutput<ItemDetails>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            bool updateActionSuccess = ItemDataManager.Update(Item);
            if (updateActionSuccess)
            {
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                //updateOperationOutput.UpdatedObject = ItemDetailMapper(Item);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }


        public ItemDetails ItemDetailMapper(Item item)
        {
            ProductManager productManager = new ProductManager();
            return new ItemDetails
            {
                ItemId = item.ProductId,
                ItemName = item.Name,
                ProductId = item.ProductId,
                ProductName = productManager.GetProductName(item.ProductId)
            };
        }


    }
}
