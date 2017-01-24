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
        [Route("GetProductFamily")]
        public ProductFamily GetProductFamily(int productFamilyId)
        {
            return _manager.GetProductFamily(productFamilyId);
        }

        [HttpPost]
        [Route("AddProductFamily")]
        public Vanrise.Entities.InsertOperationOutput<ProductFamilyDetail> AddProductFamily(ProductFamily productFamilyItem)
        {
            return _manager.AddProductFamily(productFamilyItem);
        }

        [HttpPost]
        [Route("UpdateProductFamily")]
        public Vanrise.Entities.UpdateOperationOutput<ProductFamilyDetail> UpdateProductFamily(ProductFamily productFamilyItem)
        {
            return _manager.UpdateProductFamily(productFamilyItem);
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