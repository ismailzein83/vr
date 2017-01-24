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

        [HttpPost]
        [Route("GetFilteredProductFamilies")]
        public object GetFilteredProductFamilies(Vanrise.Entities.DataRetrievalInput<ProductFamilyQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredProductFamilies(input));
        }

        [HttpGet]
        [Route("GetProductFamilyEditorRuntime")]
        public ProductFamilyEditorRuntime GetProductFamilyEditorRuntime(int productFamilyId)
        {
            return _manager.GetProductFamilyEditorRuntime(productFamilyId);
        }

        [HttpPost]
        [Route("AddProductFamily")]
        public Vanrise.Entities.InsertOperationOutput<ProductFamilyDetail> AddProductFamily(ProductFamily productFamily)
        {
            return _manager.AddProductFamily(productFamily);
        }

        [HttpPost]
        [Route("UpdateProductFamily")]
        public Vanrise.Entities.UpdateOperationOutput<ProductFamilyDetail> UpdateProductFamily(ProductFamily productFamily)
        {
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