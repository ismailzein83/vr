using System.Web.Http;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.Routing.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "ModifiedCustomerRoutePreview")]
    public class ModifiedCustomerRoutePreviewController : BaseAPIController
    {
        [HttpPost]
        [Route("GetAllModifiedCustomerRoutes")]
        public object GetAllModifiedCustomerRoutes(Vanrise.Entities.DataRetrievalInput<ModifiedCustomerRoutesPreviewQuery> input)
        {
            ModifiedCustomerRoutePreviewManager manager = new ModifiedCustomerRoutePreviewManager();
            return GetWebResponse(input, manager.GetAllModifiedCustomerRoutes(input), "Modified Customer Routes");
        }
    }
}