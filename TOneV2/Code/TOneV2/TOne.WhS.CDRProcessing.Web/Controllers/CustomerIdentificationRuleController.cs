using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.CDRProcessing.Business;
using TOne.WhS.CDRProcessing.Entities;
using Vanrise.Entities;
using Vanrise.Rules.Web.Controllers;
using Vanrise.Web.Base;

namespace TOne.WhS.CDRProcessing.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "CustomerIdentificationRule")]
    public class CustomerIdentificationRuleController : BaseRuleController<CustomerIdentificationRule, CustomerIdentificationRuleDetail, CustomerIdentificationRuleManager>
    {
        [HttpPost]
        [Route("GetFilteredCustomerIdentificationRules")]
        public object GetFilteredCustomerIdentificationRules(Vanrise.Entities.DataRetrievalInput<CustomerIdentificationRuleQuery> input)
        {
            CustomerIdentificationRuleManager manager = new CustomerIdentificationRuleManager();
            return GetWebResponse(input, manager.GetFilteredCustomerIdentificationRules(input));
        }

        [HttpPost]
        [Route("AddRule")]
        public new InsertOperationOutput<CustomerIdentificationRuleDetail> AddRule(CustomerIdentificationRule input)
        {
            return base.AddRule(input);
        }
        [HttpGet]
        [Route("GetRule")]
        public new CustomerIdentificationRule GetRule(int ruleId)
        {
            return base.GetRule(ruleId);
        }

        [HttpGet]
        [Route("DeleteRule")]
        public new DeleteOperationOutput<CustomerIdentificationRuleDetail> DeleteRule(int ruleId)
        {
            return base.DeleteRule(ruleId);
        }

        [HttpPost]
        [Route("UpdateRule")]
        public new UpdateOperationOutput<CustomerIdentificationRuleDetail> UpdateRule(CustomerIdentificationRule input)
        {
            return base.UpdateRule(input);
        }
    }
}