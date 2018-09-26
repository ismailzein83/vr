using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Vanrise.Rules.Web.Controllers;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.Sales.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "SellingRule")]
    public class SellingRuleController : BaseRuleController<SellingRule, SellingRuleDetail, SellingRuleManager>
    {
        [HttpPost]
        [Route("GetFilteredSellingRules")]
        public object GetFilteredSellingRules(Vanrise.Entities.DataRetrievalInput<SellingRuleQuery> input)
        {
            SellingRuleManager manager = new SellingRuleManager();
            return GetWebResponse(input, manager.GetFilteredSellingRules(input), "Selling Rules");
        }

        [HttpGet]
        [Route("GetSellingRuleSettingsTemplates")]
        public IEnumerable<SellingRuleSettingsConfig> GetSellingRuleSettingsTemplates()
        {
            SellingRuleManager manager = new SellingRuleManager();
            return manager.GetSellingRuleTypesTemplates();
        }

        [HttpGet]
        [Route("GetRule")]
        public new SellingRule GetRule(int ruleId)
        {
            return base.GetRule(ruleId);
        }

        [HttpPost]
        [Route("AddRule")]
        public new InsertOperationOutput<SellingRuleDetail> AddRule(SellingRule input)
        {
            return base.AddRule(input);
        }

        [HttpPost]
        [Route("UpdateRule")]
        public new UpdateOperationOutput<SellingRuleDetail> UpdateRule(SellingRule input)
        {
            return base.UpdateRule(input);
        }

        [HttpGet]
        [Route("DeleteRule")]
        public DeleteOperationOutput<SellingRuleDetail> DeleteRule(int ruleId)
        {
            return base.SetRuleDeleted(ruleId);
        }
    }
}