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
        ProductDefinitionManager _defManager = new ProductDefinitionManager();
        ProductFamilyManager _familyManager = new ProductFamilyManager();

        [HttpPost]
        [Route("GetFilteredProducts")]
        public object GetFilteredProducts(Vanrise.Entities.DataRetrievalInput<ProductQuery> input)
        {
            if (!_defManager.DoesUserHaveViewProductDefinitions())
                return GetUnauthorizedResponse();
            return GetWebResponse(input, _manager.GetFilteredProducts(input));
        }

        [HttpGet]
        [Route("GetProductEditorRuntime")]
        public ProductEditorRuntime GetProductEditorRuntime(int productId)
        {
            return _manager.GetProductEditorRuntime(productId);
        }
        [HttpGet]
        [Route("DoesUserHaveAddAccess")]
        public bool DoesUserHaveAddAccess()
        {
            return _defManager.DoesUserHaveAddProductDefinitions();
        }
        [HttpPost]
        [Route("AddProduct")]
        public object AddProduct(Product product)
        {
            if (!_familyManager.DoesUserHaveAddProductDefinitions(product.Settings.ProductFamilyId))
                return GetUnauthorizedResponse();
            return _manager.AddProduct(product);
        }

        [HttpPost]
        [Route("UpdateProduct")]
        public object UpdateProduct(Product product)
        {
            if (!_familyManager.DoesUserHaveEditProductDefinitions(product.Settings.ProductFamilyId))
                return GetUnauthorizedResponse();
            return _manager.UpdateProduct(product);
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