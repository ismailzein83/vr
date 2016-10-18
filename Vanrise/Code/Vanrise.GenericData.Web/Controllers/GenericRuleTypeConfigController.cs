using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Web.Base;

namespace Vanrise.GenericData.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "GenericRuleTypeConfig")]
    public class GenericRuleTypeConfigController:BaseAPIController
    {
        [HttpGet]
        [Route("GetGenericRuleTypes")]
        public IEnumerable<GenericRuleTypeConfig> GetGenericRuleTypes()
        {
            GenericRuleTypeConfigManager manager = new GenericRuleTypeConfigManager();
            return manager.GetGenericRuleTypes();
        }

        [HttpGet]
        [Route("GetGenericRuleTypeByName")]
        public GenericRuleTypeConfig GetGenericRuleTypeByName(string ruleTypeName)
        {
            GenericRuleTypeConfigManager manager = new GenericRuleTypeConfigManager();
            return manager.GetGenericRuleTypeByName(ruleTypeName);
        }

        [HttpGet]
        [Route("GetGenericRuleTypeById")]
        public GenericRuleTypeConfig GetGenericRuleTypeById(Guid ruleTypeConfigId)
        {
            GenericRuleTypeConfigManager manager = new GenericRuleTypeConfigManager();
            return manager.GetGenericRuleTypeById(ruleTypeConfigId);
        }

    }
}