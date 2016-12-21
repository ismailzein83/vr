using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "AccountDefinition")]
    public class AccountDefinitionController : BaseAPIController
    {
        AccountDefinitionManager _manager = new AccountDefinitionManager();

        [HttpGet]
        [Route("GetAccountViewDefinitionSettingsConfigs")]
        public IEnumerable<AccountViewDefinitionSettingsConfig> GetAccountViewDefinitionSettingsConfigs()
        {
            return _manager.GetAccountViewDefinitionSettingsConfigs();
        }

        [HttpGet]
        [Route("GetAccountConditionConfigs")]
        public IEnumerable<AccountConditionConfig> GetAccountConditionConfigs()
        {
            return _manager.GetAccountConditionConfigs();
        }
    }
}