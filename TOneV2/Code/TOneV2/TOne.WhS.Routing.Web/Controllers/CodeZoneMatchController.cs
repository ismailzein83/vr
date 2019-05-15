using System.Collections.Generic;
using System.Web.Http;
using TOne.WhS.Routing.Business;
using Vanrise.Web.Base;

namespace TOne.WhS.Routing.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "CodeZoneMatch")]
    public class CodeZoneMatchController : BaseAPIController
    {
        [HttpPost]
        [Route("GetSaleZonesMatchingSpecificDeals")]
        public List<long> GetSaleZonesMatchingSupplierDeals(GetSaleZonesMatchingSpecificDealsInput input)
        {
            CodeZoneMatchManager _manager = new CodeZoneMatchManager();
            return _manager.GetSaleZonesMatchingSupplierDeals(input.SelectedSupplierDealIds, input.SellingNumberPlanId, input.SelectedSaleZoneIds);
        }
    }

    public class GetSaleZonesMatchingSpecificDealsInput
    {
        public List<int> SelectedSupplierDealIds { get; set; }

        public List<long> SelectedSaleZoneIds { get; set; }

        public int SellingNumberPlanId { get; set; }
    }
}