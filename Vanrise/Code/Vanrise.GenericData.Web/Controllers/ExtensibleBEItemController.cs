using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Web.Base;

namespace Vanrise.GenericData.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "ExtensibleBEItem")]
    public class ExtensibleBEItemController : BaseAPIController
    {
        [HttpGet]
        [Route("GetExtensibleBEItem")]
        public ExtensibleBEItem GetExtensibleBEItem(Guid extensibleBEItemId)
        {
            ExtensibleBEItemManager manager = new ExtensibleBEItemManager();
            return manager.GetExtensibleBEItem(extensibleBEItemId);
        }
        [Route("UpdateExtensibleBEItem")]
        public Vanrise.Entities.UpdateOperationOutput<ExtensibleBEItemDetail> UpdateExtensibleBEItem(ExtensibleBEItem extensibleBEItem)
        {
            ExtensibleBEItemManager manager = new ExtensibleBEItemManager();
            return manager.UpdateExtensibleBEItem(extensibleBEItem);
        }

        [HttpPost]
        [Route("AddExtensibleBEItem")]
        public Vanrise.Entities.InsertOperationOutput<ExtensibleBEItemDetail> AddExtensibleBEItem(ExtensibleBEItem extensibleBEItem)
        {
            ExtensibleBEItemManager manager = new ExtensibleBEItemManager();
            return manager.AddExtensibleBEItem(extensibleBEItem);
        }
        [HttpPost]
        [Route("GetFilteredExtensibleBEItems")]
        public object GetFilteredExtensibleBEItems(Vanrise.Entities.DataRetrievalInput<ExtensibleBEItemQuery> input)
        {
            ExtensibleBEItemManager manager = new ExtensibleBEItemManager();
            return GetWebResponse(input, manager.GetFilteredExtensibleBEItems(input));
        }
    }
}