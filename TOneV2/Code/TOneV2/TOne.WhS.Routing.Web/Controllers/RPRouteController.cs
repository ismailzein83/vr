using System.Collections.Generic;
using System.Web.Http;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.Routing.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "RPRoute")]
    public class RPRouteController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredRPRoutesByZone")]
        public object GetFilteredRPRoutes(Vanrise.Entities.DataRetrievalInput<RPRouteQueryByZone> input)
        {
            RPRouteManager manager = new RPRouteManager();
            return GetWebResponse(input, manager.GetFilteredRPRoutesByZone(input), "Cost Analysis");
        }

        [HttpPost]
        [Route("GetFilteredRPRoutesByCode")]
        public object GetFilteredRPRoutesByCode(Vanrise.Entities.DataRetrievalInput<RPRouteQueryByCode> input)
        {
            RPRouteManager manager = new RPRouteManager();
            return GetWebResponse(input, manager.GetFilteredRPRoutesByCode(input), "RP Routes By Code");
        }

        [HttpPost]
        [Route("GetRPRouteOptionSupplier")]
        public RPRouteOptionSupplierDetail GetRPRouteOptionSupplier(RPRouteOptionSupplierInput rpRouteOptionSupplierInput)
        {
            RPRouteManager manager = new RPRouteManager();
            return manager.GetRPRouteOptionSupplier(rpRouteOptionSupplierInput);
        }

        [HttpGet]
        [Route("GetPoliciesOptionTemplates")]
        public IEnumerable<RPRouteOptionPolicySetting> GetPoliciesOptionTemplates(string filter = null)
        {
            RPRouteManager manager = new RPRouteManager();
            var deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<RPRouteOptionPolicyFilter>(filter) : null;
            return manager.GetPoliciesOptionTemplates(deserializedFilter);
        }

        [HttpPost]
        [Route("GetFilteredRPRouteOptions")]
        public object GetFilteredRPRouteOptions(Vanrise.Entities.DataRetrievalInput<RPRouteOptionQuery> input)
        {
            RPRouteManager manager = new RPRouteManager();
            return GetWebResponse(input, manager.GetFilteredRPRouteOptions(input), "RP Route Options");
        }
    }
}