﻿using System;
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
        [Route("GetRatePlanPreviewSummary")]
        public RatePlanPreviewSummary GetRatePlanPreviewSummary(RatePlanPreviewQuery query)
        {
            var manager = new RatePlanPreviewSummaryManager();
            return manager.GetRatePlanPreviewSummary(query);
        }

        [HttpPost]
        [Route("GetFilteredRatePreviews")]
        public object GetFilteredRatePreviews(Vanrise.Entities.DataRetrievalInput<RatePreviewQuery> input)
        {
            var manager = new RatePreviewManager();
            return GetWebResponse(input, manager.GetFilteredRatePreviews(input));
        }

        [HttpPost]
        [Route("GetFilteredSaleZoneRoutingProductPreviews")]
        public object GetFilteredSaleZoneRoutingProductPreviews(Vanrise.Entities.DataRetrievalInput<RatePlanPreviewQuery> input)
        {
            var manager = new SaleZoneRoutingProductPreviewManager();
            return GetWebResponse(input, manager.GetFilteredSaleZoneRoutingProductPreviews(input));
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
            return GetWebResponse(input, manager.GetFilteredSaleZoneServicePreviews(input));
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
			return GetWebResponse(input, manager.GetFilteredChangedCustomerCountryPreviews(input));
		}

		[HttpPost]
		[Route("GetFilteredNewCustomerCountryPreviews")]
		public object GetFilteredNewCustomerCountryPreviews(Vanrise.Entities.DataRetrievalInput<RatePlanPreviewQuery> input)
		{
			var manager = new NewCustomerCountryPreviewManager();
			return GetWebResponse(input, manager.GetFilteredNewCustomerCountryPreviews(input));
		}
    }
}