using Demo.Module.Business;
using Demo.Module.Entities;
using Demo.Module.Entities.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Item")]
      [JSONWithType]
    public class ItemController : BaseAPIController
    {
        [HttpPost]
        [Route("AddItem")]
        public InsertOperationOutput<ItemDetails> AddItem(Item item)
        {
            ItemManager itemManager = new ItemManager();
            return itemManager.AddItem(item);
        }

        [HttpPost]
        [Route("UpdateItem")]
        public UpdateOperationOutput<ItemDetails> UpdateItem(Item item)
        {
            ItemManager itemManager = new ItemManager();
            return itemManager.UpdateItem(item);
        }

        [HttpPost]
        [Route("GetFilteredItems")]
        public object GetFilteredItems(DataRetrievalInput<ItemQuery> input)
        {
            ItemManager itemManager = new ItemManager();
            return GetWebResponse(input, itemManager.GetFilteredItems(input));
        }

    
        [HttpGet]
        [Route("GetItemById")]
        public Item GetItemById(int itemId)
        {
            ItemManager itemManager = new ItemManager();
            return itemManager.GetItemById(itemId);
        }

        //[HttpGet]
        //[Route("DeleteItem")]
        //public DeleteOperationOutput<ItemDetails> DeleteItem(int itemId)
        //{
        //    ItemManager itemManager = new ItemManager();
        //    return itemManager.Delete(itemId);
        //}
    }
}