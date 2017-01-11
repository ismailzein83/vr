using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.Web.Controllers
{

    [RoutePrefix(Constants.ROUTE_PREFIX + "Product")]
    [JSONWithTypeAttribute]
    public class ProductController : BaseAPIController
    {
        ProductManager _manager = new ProductManager();

        [HttpPost]
        [Route("GetFilteredProducts")]
        public object GetFilteredProducts(Vanrise.Entities.DataRetrievalInput<ProductQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredProducts(input));
        }

        [HttpGet]
        [Route("GetProductEditorRuntime")]
        public ProductEditorRuntime GetProductEditorRuntime(int productId)
        {
            return _manager.GetProductEditorRuntime(productId);
        }

        [HttpPost]
        [Route("AddProduct")]
        public Vanrise.Entities.InsertOperationOutput<ProductDetail> AddProduct(Product productItem)
        {
            return _manager.AddProduct(productItem);
        }

        [HttpPost]
        [Route("UpdateProduct")]
        public Vanrise.Entities.UpdateOperationOutput<ProductDetail> UpdateProduct(Product productItem)
        {
            return _manager.UpdateProduct(productItem);
        }

        [HttpGet]
        [Route("GetProductsInfo")]
        public IEnumerable<ProductInfo> GetProductsInfo(string serializedFilter = null)
        {
            ProductInfoFilter deserializedFilter = (serializedFilter != null) ? Vanrise.Common.Serializer.Deserialize<ProductInfoFilter>(serializedFilter) : null;
            return _manager.GetProductsInfo(deserializedFilter);
        }
    }
}