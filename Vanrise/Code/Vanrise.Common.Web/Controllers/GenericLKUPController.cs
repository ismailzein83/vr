using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "GenericLKUP")]
    [JSONWithTypeAttribute]
    public class GenericLKUPController : BaseAPIController
    {
        GenericLKUPManager _manager = new GenericLKUPManager();

        [HttpPost]
        [Route("GetFilteredGenericLKUPItems")]
        public object GetFilteredGenericLKUPItems(Vanrise.Entities.DataRetrievalInput<GenericLKUPQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredGenericLKUPItems(input));
        }

        [HttpPost]
        [Route("AddGenericLKUPItem")]
        public Vanrise.Entities.InsertOperationOutput<GenericLKUPItemDetail> AddGenericLKUPItem(GenericLKUPItem genericLKUPItem)
        {
            return _manager.AddGenericLKUPItem(genericLKUPItem);
        }

        [HttpPost]
        [Route("UpdateGenericLKUPItem")]
        public Vanrise.Entities.UpdateOperationOutput<GenericLKUPItemDetail> UpdateGenericLKUPItem(GenericLKUPItem genericLKUPItem)
        {
            return _manager.UpdateGenericLKUPItem(genericLKUPItem);
        }

        [HttpGet]
        [Route("GetGenericLKUPItem")]
        public GenericLKUPItem GetGenericLKUPItem(Guid genericLKUPItemId)
        {
            return _manager.GetGenericLKUPItem(genericLKUPItemId);
        }

        [HttpGet]
        [Route("GetGenericLKUPDefinitionExtendedSetings")]
        public GenericLKUPDefinitionExtendedSettings GetGenericLKUPDefinitionExtendedSetings(Guid businessEntityDefinitionId)
        {
            GenericLKUPDefinitionManager manager = new GenericLKUPDefinitionManager();
            return manager.GetGenericLKUPDefinitionExtendedSetings(businessEntityDefinitionId);
        }

        [HttpGet]
        [Route("GetGenericLKUPItemsInfo")]
        public IEnumerable<GenericLKUPItemInfo> GetGenericLKUPItemsInfo(string filter = null)
        {
            GenericLKUPItemInfoFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<GenericLKUPItemInfoFilter>(filter) : null;
            return _manager.GetGenericLKUPItemsInfo(deserializedFilter);
        }
      
    }
}