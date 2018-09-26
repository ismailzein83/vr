using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.Sales.Web.Controllers
{
    [JSONWithType]
    [RoutePrefix(Constants.ROUTE_PREFIX + "RatePlanPreview")]
    public class RatePlanPreviewController : BaseAPIController
    {
        [HttpPost]
        [Route("GetCustomerRatePlanPreviewSummary")]
        public RatePlanPreviewSummary GetCustomerRatePlanPreviewSummary(RatePlanPreviewQuery query)
        {
            var manager = new RatePlanPreviewSummaryManager();
            return manager.GetCustomerRatePlanPreviewSummary(query);
        }
         [HttpPost]
         [Route("GetProductRatePlanPreviewSummary")]
        public RatePlanPreviewSummary GetProductRatePlanPreviewSummary(RatePlanPreviewQuery query)
        {
            var manager = new RatePlanPreviewSummaryManager();
            return manager.GetProductRatePlanPreviewSummary(query);
        }
        
        [HttpPost]
        [Route("GetFilteredRatePreviews")]
        public object GetFilteredRatePreviews(Vanrise.Entities.DataRetrievalInput<RatePreviewQuery> input)
        {
            var manager = new RatePreviewManager();
            return GetWebResponse(input, manager.GetFilteredRatePreviews(input), "Rate Previews");
        }

        [HttpGet]
        [Route("GetSubscriberPreviews")]
        public SubscriberPreviewObject GetSubscriberPreviews(long processInstanceId)
        {
            var manager = new SubscriberPreviewManager();
            return manager.GetSubscriberPreview(processInstanceId);
        }

        [HttpPost]
        [Route("GetFilteredSaleZoneRoutingProductPreviews")]
        public object GetFilteredSaleZoneRoutingProductPreviews(Vanrise.Entities.DataRetrievalInput<RatePlanPreviewQuery> input)
        {
            var manager = new SaleZoneRoutingProductPreviewManager();
            return GetWebResponse(input, manager.GetFilteredSaleZoneRoutingProductPreviews(input), "Sale Zone Routing Product Previews");
        }

        [HttpPost]
        [Route("GetDefaultRoutingProductPreview")]
        public DefaultRoutingProductPreview GetDefaultRoutingProductPreview(RatePlanPreviewQuery query)
        {
            var manager = new DefaultRoutingProductPreviewManager();
            return manager.GetDefaultRoutingProductPreview(query);
        }

        [HttpPost]
        [Route("GetFilteredSaleZoneServicePreviews")]
        public object GetFilteredSaleZoneServicePreviews(Vanrise.Entities.DataRetrievalInput<RatePlanPreviewQuery> input)
        {
            var manager = new SaleZoneServicePreviewManager();
            return GetWebResponse(input, manager.GetFilteredSaleZoneServicePreviews(input), "Sale Zone Service Previews");
        }

        [HttpPost]
        [Route("GetDefaultServicePreview")]
        public DefaultServicePreview GetDefaultServicePreview(RatePlanPreviewQuery query)
        {
            var manager = new DefaultServicePreviewManager();
            return manager.GetDefaultServicePreview(query);
        }

		[HttpPost]
		[Route("GetFilteredChangedCustomerCountryPreviews")]
		public object GetFilteredChangedCustomerCountryPreviews(Vanrise.Entities.DataRetrievalInput<RatePlanPreviewQuery> input)
		{
			var manager = new ChangedCustomerCountryPreviewManager();
            return GetWebResponse(input, manager.GetFilteredChangedCustomerCountryPreviews(input), "Changed Customer Country Previews");
		}

		[HttpPost]
		[Route("GetFilteredNewCustomerCountryPreviews")]
		public object GetFilteredNewCustomerCountryPreviews(Vanrise.Entities.DataRetrievalInput<RatePlanPreviewQuery> input)
		{
			var manager = new NewCustomerCountryPreviewManager();
            return GetWebResponse(input, manager.GetFilteredNewCustomerCountryPreviews(input), "New Customer Country Previews");
		}
    }
}