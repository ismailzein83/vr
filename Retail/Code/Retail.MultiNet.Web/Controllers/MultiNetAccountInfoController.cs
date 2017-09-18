using Retail.MultiNet.APIEntities;
using Retail.MultiNet.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.MultiNet.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "MultiNetAccountInfo")]
    [JSONWithTypeAttribute]
    public class MultiNetAccountInfoController : BaseAPIController
    {
        MultiNetAccountManager _manager = new MultiNetAccountManager();
        [HttpGet]
        [Route("GetClientAccountAdditionalInfo")]
        public ClientAccountAdditionalInfo GetClientAccountAdditionalInfo(Guid accountBEDefinitionId, long accountId)
        {
            return _manager.GetClientAccountAdditionalInfo(accountBEDefinitionId, accountId);
        }
    }
}