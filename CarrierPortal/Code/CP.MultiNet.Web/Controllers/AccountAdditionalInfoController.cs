using CP.MultiNet.Business;
using Retail.MultiNet.APIEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace CP.MultiNet.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "AccountAdditionalInfo")]
    [JSONWithTypeAttribute]
    public class AccountAdditionalInfoController: BaseAPIController
    {
        AccountAdditionalInfoManager _manager = new AccountAdditionalInfoManager();
        [HttpGet]
        [Route("GetClientAccountAdditionalInfo")]
        public ClientAccountAdditionalInfo GetClientAccountAdditionalInfo(Guid vrConnectionId)
        {
            return _manager.GetClientAccountAdditionalInfo(vrConnectionId);
        }
    }
}