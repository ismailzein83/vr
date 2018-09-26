using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "RoutingProduct")]
    public class RoutingProductController : BaseAPIController
    {
        RoutingProductManager _manager;
        public RoutingProductController()
        {
            _manager = new RoutingProductManager();
        }

        [HttpPost]
        [Route("GetFilteredRoutingProducts")]
        public object GetFilteredRoutingProducts(Vanrise.Entities.DataRetrievalInput<RoutingProductQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredRoutingProducts(input), "Routing Products");
        }

        [HttpGet]
        [Route("GetRoutingProductInfo")]
        public IEnumerable<RoutingProductInfo> GetRoutingProductInfo(string filter)
        {
            RoutingProductInfoFilter deserializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<RoutingProductInfoFilter>(filter) : null;
            return _manager.GetRoutingProductInfo(deserializedFilter);
        }
        [HttpGet]
        public IEnumerable<RoutingProductInfo> GetRoutingProductsInfoBySellingNumberPlan(int sellingNumberPlanId)
        {
            return _manager.GetRoutingProductsInfoBySellingNumberPlan(sellingNumberPlanId);
        }
        
        [HttpGet]
        [Route("GetRoutingProduct")]
        public RoutingProduct GetRoutingProduct(int routingProductId)
        {
            return _manager.GetRoutingProduct(routingProductId,true);
        }

        [HttpGet]
        [Route("GetRoutingProductEditorRuntime")]
        public RoutingProductEditorRuntime GetRoutingProductEditorRuntime(int routingProductId)
        {
            return _manager.GetRoutingProductEditorRuntime(routingProductId);
        }

        [HttpPost]
        [Route("AddRoutingProduct")]
        public InsertOperationOutput<RoutingProductDetail> AddRoutingProduct(RoutingProduct routingProduct)
        {
            return _manager.AddRoutingProduct(routingProduct);
        }

        [HttpPost]
        [Route("UpdateRoutingProduct")]
        public UpdateOperationOutput<RoutingProductDetail> UpdateRoutingProduct(RoutingProductToEdit routingProduct)
        {
            return _manager.UpdateRoutingProduct(routingProduct);
        }

        [HttpGet]
        [Route("DeleteRoutingProduct")]
        public DeleteOperationOutput<object> DeleteRoutingProduct(int routingProductId)
        {
            return _manager.DeleteRoutingProduct(routingProductId);
        }

        [HttpGet]
        [Route("CheckIfRoutingProductHasRelatedSaleEntities")]
        public bool CheckIfRoutingProductHasRelatedSaleEntities(int routingProductId)
        {
            return _manager.CheckIfRoutingProductHasRelatedSaleEntities(routingProductId);
        } 
    }
}