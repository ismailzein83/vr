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
    [RoutePrefix(Constants.ROUTE_PREFIX + "SellingProduct")]
    [JSONWithTypeAttribute]
    public class SellingProductController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredSellingProducts")]
        public object GetFilteredSellingProducts(Vanrise.Entities.DataRetrievalInput<SellingProductQuery> input)
        {
            SellingProductManager manager = new SellingProductManager();
            return GetWebResponse(input, manager.GetFilteredSellingProducts(input), "Selling Products");
        }

        [HttpGet]
        [Route("GetSellingProductHistoryDetailbyHistoryId")]
        public SellingProduct GetSellingProductHistoryDetailbyHistoryId(int sellingProductHistoryId)
        {
            SellingProductManager manager = new SellingProductManager();
            return manager.GetSellingProductHistoryDetailbyHistoryId(sellingProductHistoryId);
        }
        [HttpGet]
        [Route("GetSellingProductsInfo")]
        public IEnumerable<SellingProductInfo> GetSellingProductsInfo(string serializedFilter)
        {
            SellingProductInfoFilter filter = serializedFilter != null ? Vanrise.Common.Serializer.Deserialize<SellingProductInfoFilter>(serializedFilter) : null;
            SellingProductManager manager = new SellingProductManager();
            return manager.GetSellingProductsInfo(filter);
        }

        [HttpGet]
        [Route("GetSellingProduct")]
        public SellingProduct GetSellingProduct(int sellingProductId)
        {
            SellingProductManager manager = new SellingProductManager();
            return manager.GetSellingProduct(sellingProductId, true);
        }

        [HttpPost]
        [Route("AddSellingProduct")]
        public InsertOperationOutput<SellingProductDetail> AddSellingProduct(SellingProduct sellingProduct)
        {
            SellingProductManager manager = new SellingProductManager();
            return manager.AddSellingProduct(sellingProduct);
        }

        [HttpPost]
        [Route("UpdateSellingProduct")]
        public UpdateOperationOutput<SellingProductDetail> UpdateSellingProduct(SellingProductToEdit sellingProduct)
        {
            SellingProductManager manager = new SellingProductManager();
            return manager.UpdateSellingProduct(sellingProduct);
        }

        [HttpGet]
        [Route("GetSellingProductName")]
        public string GetSellingProductName(int sellingProductId)
        {
            SellingProductManager manager = new SellingProductManager();
            return manager.GetSellingProductName(sellingProductId);
        }
        [HttpGet]
        [Route("GetSellingProductCurrencyId")]
        public int GetSellingProductCurrencyId(int sellingProductId)
        {
            SellingProductManager manager = new SellingProductManager();
            return manager.GetSellingProductCurrencyId(sellingProductId);
        }

        [HttpGet]
        [Route("GetSellingProductPricingSettings")]
        public PricingSettings GetSellingProductPricingSettings(int sellingProductId)
        {
            return new SellingProductManager().GetSellingProductPricingSettings(sellingProductId);
        }
    }
}