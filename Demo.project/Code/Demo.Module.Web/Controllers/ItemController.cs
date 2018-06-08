using Demo.Module.Business;
using Demo.Module.Entities;
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
        ItemManager itemManager = new ItemManager();

        [HttpPost]
        [Route("AddItem")]
        public InsertOperationOutput<ItemDetails> AddItem(Item item)
        {
            return itemManager.AddItem(item);
        }

        [HttpPost]
        [Route("UpdateItem")]
        public UpdateOperationOutput<ItemDetails> UpdateItem(Item item)
        {
            return itemManager.UpdateItem(item);
        }

        [HttpPost]
        [Route("GetFilteredItems")]
        public object GetFilteredItems(DataRetrievalInput<ItemQuery> input)
        {
            return GetWebResponse(input, itemManager.GetFilteredItems(input));
        }

    
        [HttpGet]
        [Route("GetItemById")]
        public Item GetItemById(int itemId)
        {
            return itemManager.GetItemById(itemId);
        }

        [HttpGet]
        [Route("GetItemShapeConfigs")]
        public IEnumerable<ItemShapeConfig> GetItemShapeConfigs()
        {
            return itemManager.GetItemShapeConfigs();
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