using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "SupplierRate")]
    public class SupplierRateController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredSupplierRates")]
        public object GetFilteredSupplierRates(Vanrise.Entities.DataRetrievalInput<BaseSupplierRateQueryHandler> input)
        {
            SupplierRateManager manager = new SupplierRateManager();
            return GetWebResponse(input, manager.GetFilteredSupplierRates(input), "Supplier Rates");
        }
    }

}