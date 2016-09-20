using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;
using Vanrise.Rules.Web.Controllers;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.Routing.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "RouteOptionRule")]
    [JSONWithTypeAttribute]

    public class RouteOptionRuleController : BaseRuleController<RouteOptionRule, RouteOptionRuleDetail, RouteOptionRuleManager>
    {
        [HttpPost]
        [Route("GetFilteredRouteOptionRules")]
        public object GetFilteredRouteOptionRules(Vanrise.Entities.DataRetrievalInput<RouteOptionRuleQuery> input)
        {
            RouteOptionRuleManager manager = new RouteOptionRuleManager();
            return GetWebResponse(input, manager.GetFilteredRouteOptionRules(input));
        }

        [HttpGet]
        [Route("GetRouteOptionRuleSettingsTemplates")]
        public IEnumerable<RouteOptionRuleConfig> GetRouteOptionRuleSettingsTemplates()
        {
            RouteOptionRuleManager manager = new RouteOptionRuleManager();
            return manager.GetRouteOptionRuleTypesTemplates();
        }

        [HttpGet]
        [Route("GetRule")]
        public new RouteOptionRule GetRule(int ruleId)
        {
            return base.GetRule(ruleId);
        }

        [HttpPost]
        [Route("AddRule")]
        public new InsertOperationOutput<RouteOptionRuleDetail> AddRule(RouteOptionRule input)
        {
            return base.AddRule(input);
        }

        [HttpPost]
        [Route("UpdateRule")]
        public new UpdateOperationOutput<RouteOptionRuleDetail> UpdateRule(RouteOptionRule input)
        {
            return base.UpdateRule(input);
        }

        [HttpGet]
        [Route("DeleteRule")]
        public new DeleteOperationOutput<RouteOptionRuleDetail> DeleteRule(int ruleId)
        {
            return base.DeleteRule(ruleId);
        }


    }
}