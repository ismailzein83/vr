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
    public class GenericRuleTypeConfigController
    {
        [HttpGet]
        [Route("GetGenericRuleTypes")]
        public IEnumerable<GenericRuleTypeConfig> GetGenericRuleTypes()
        {
            GenericRuleTypeConfigManager manager = new GenericRuleTypeConfigManager();
            return manager.GetGenericRuleTypes();
        }

        [HttpGet]
        [Route("GetGenericRuleType")]
        public GenericRuleTypeConfig GetGenericRuleType(int configId)
        {
            GenericRuleTypeConfigManager manager = new GenericRuleTypeConfigManager();
            return manager.GetGenericRuleType(configId);
        }

    }
}