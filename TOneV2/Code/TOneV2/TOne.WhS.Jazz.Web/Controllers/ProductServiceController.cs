using System.Collections.Generic;
using Vanrise.Web.Base;
using System.Web.Http;
using TOne.WhS.Jazz.Business;
using TOne.WhS.Jazz.Entities;
namespace TOne.WhS.Jazz.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "ProductService")]
    public class ProductServiceController : BaseAPIController
    {
        ProductServiceManager _manager = new ProductServiceManager();

        [HttpGet]
        [Route("GetProductServicesInfo")]
        public IEnumerable<ProductServiceDetail> GetProductServicesInfo(string filter=null)
        {
            ProductServiceInfoFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<ProductServiceInfoFilter>(filter) : null;
            return _manager.GetProductServicesInfo(deserializedFilter);
        }

        [HttpGet]
        [Route("GetAllProductServices")]
        public IEnumerable<ProductService> GetAllProductServices()
        {
            return _manager.GetAllProductServices();
        }
    }
}