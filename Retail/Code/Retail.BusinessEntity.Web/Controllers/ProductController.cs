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
        [Route("GetProduct")]
        public Product GetProduct(int productId)
        {
            return _manager.GetProduct(productId);
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

        //[HttpGet]
        //[Route("GetProductesInfo")]
        //public IEnumerable<ProductInfo> GetProductesInfo(string filter = null)
        //{
        //    ProductFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<ProductFilter>(filter) : null;
        //    return _manager.GetProductesInfo(deserializedFilter);
        //}
    }
}