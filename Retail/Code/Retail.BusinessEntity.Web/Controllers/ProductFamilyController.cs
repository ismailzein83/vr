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
    [RoutePrefix(Constants.ROUTE_PREFIX + "ProductFamily")]
    [JSONWithTypeAttribute]
    public class ProductFamilyFamilyController : BaseAPIController
    {
        ProductFamilyManager _manager = new ProductFamilyManager();
        ProductDefinitionManager _defManager = new ProductDefinitionManager();

        [HttpPost]
        [Route("GetFilteredProductFamilies")]
        public object GetFilteredProductFamilies(Vanrise.Entities.DataRetrievalInput<ProductFamilyQuery> input)
        {
            if (!_defManager.DoesUserHaveViewProductDefinitions())
                return GetUnauthorizedResponse();
            return GetWebResponse(input, _manager.GetFilteredProductFamilies(input));
        }

        [HttpGet]
        [Route("GetProductFamilyEditorRuntime")]
        public ProductFamilyEditorRuntime GetProductFamilyEditorRuntime(int productFamilyId)
        {
            return _manager.GetProductFamilyEditorRuntime(productFamilyId);
        }

        [HttpGet]
        [Route("DoesUserHaveAddAccess")]
        public bool DoesUserHaveAddAccess()
        {
            return _defManager.DoesUserHaveAddProductDefinitions();
        }

        [HttpPost]
        [Route("AddProductFamily")]
        public object AddProductFamily(ProductFamily productFamily)
        {
            if (!_defManager.DoesUserHaveAddProductDefinitions(productFamily.Settings.ProductDefinitionId))
                return GetUnauthorizedResponse();

            return _manager.AddProductFamily(productFamily);
        }

        [HttpPost]
        [Route("UpdateProductFamily")]
        public object UpdateProductFamily(ProductFamily productFamily)
        {
            if (!_defManager.DoesUserHaveEditProductDefinitions(productFamily.Settings.ProductDefinitionId))
                return GetUnauthorizedResponse();
            return _manager.UpdateProductFamily(productFamily);
        }

        [HttpGet]
        [Route("GetProductFamiliesInfo")]
        public IEnumerable<ProductFamilyInfo> GetProductFamiliesInfo(string serializedFilter = null)
        {
            ProductFamilyFilter deserializedFilter = (serializedFilter != null) ? Vanrise.Common.Serializer.Deserialize<ProductFamilyFilter>(serializedFilter) : null;
            return _manager.GetProductFamiliesInfo(deserializedFilter);
        }
    }
}