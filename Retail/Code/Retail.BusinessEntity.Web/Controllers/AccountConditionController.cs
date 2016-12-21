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
    [RoutePrefix(Constants.ROUTE_PREFIX + "AccountCondition")]
    public class AccountConditionController : BaseAPIController
    {
        AccountConditionManager _manager = new AccountConditionManager();

        [HttpGet]
        [Route("GetAccountConditionConfigs")]
        public IEnumerable<AccountConditionConfig> GetAccountConditionConfigs()
        {
            return _manager.GetAccountConditionConfigs();
        }
    }
}