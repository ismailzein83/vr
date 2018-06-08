using Demo.Module.Business;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;


namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRProduct")]
    [JSONWithTypeAttribute]
    public class VRProductController : BaseAPIController
    {
        ProductManager productManager = new ProductManager();
        [HttpPost]
        [Route("GetFilteredProducts")]
        public object GetFilteredProducts(DataRetrievalInput<ProductQuery> input)
        {
            return GetWebResponse(input, productManager.GetFilteredProducts(input));
        }

        [HttpGet]
        [Route("GetProductById")]
        public Product GetProductById(long productId)
        {
            return productManager.GetProductById(productId);
        }

        [HttpPost]
        [Route("UpdateProduct")]
        public UpdateOperationOutput<ProductDetails> UpdateProduct(Product product)
        {
            return productManager.UpdateProduct(product);
        }

        [HttpPost]
        [Route("AddProduct")]
        public InsertOperationOutput<ProductDetails> AddProduct(Product product)
        {
            return productManager.AddProduct(product);
        }

        [HttpGet]
        [Route("GetProductsInfo")]
        public IEnumerable<Demo.Module.Entities.ProductInfo.ProductInfo> GetProductsInfo(string filter = null)
        {
            ProductInfoFilter parentInfoFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<ProductInfoFilter>(filter) : null;
            return productManager.GetProductsInfo(parentInfoFilter);
        }
    }
}