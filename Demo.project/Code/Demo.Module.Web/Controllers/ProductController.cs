using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Demo.Module.Business;
using Demo.Module.Entities;
using Vanrise.Web.Base;
using Vanrise.Entities;

namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Demo_Product")]
    [JSONWithTypeAttribute]
    public class ProductController : BaseAPIController
    {
        ProductManager _productManager = new ProductManager();

        [HttpPost]
        [Route("GetFilteredProducts")]
        public object GetFilteredProducts(DataRetrievalInput<ProductQuery> input)
        {
            return GetWebResponse(input, _productManager.GetFilteredProducts(input));
        }

        [HttpGet]
        [Route("GetProductById")]
        public Product GetProductById(long productId)
        {
            return _productManager.GetProductById(productId);
        }

        [HttpPost]
        [Route("AddProduct")]
        public InsertOperationOutput<ProductDetail> AddProduct(Product product)
        {
            return _productManager.AddProduct(product);
        }

        [HttpPost]
        [Route("UpdateProduct")]
        public UpdateOperationOutput<ProductDetail> UpdateProduct(Product product)
        {
            return _productManager.UpdateProduct(product);
        }

        [HttpGet]
        [Route("GetProductSettingsConfigs")]
        public IEnumerable<ProductSettingsConfig> GetProductSettingsConfigs()
        {
            return _productManager.GetProductSettingsConfigs();
        }

        [HttpGet]
        [Route("GetOperatingSystemConfigs")]
        public IEnumerable<SoftwareOperatingSystemConfig> GetOperatingSystemConfigs()
        {
            return _productManager.GetOperatingSystemConfigs();
        }
    }
}
