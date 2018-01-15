using Retail.Teles.Business;
using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.Teles.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "TelesAccount")]
    public class TelesAccountController:BaseAPIController
    {
        TelesAccountManager _manager = new TelesAccountManager();

        [HttpGet]
        [Route("GetAccountTelesInfo")]
        public TelesAccountInfo GetAccountTelesInfo(Guid accountBEDefinitionId, long accountId, Guid vrConnectionId)
        {
            return _manager.GetAccountTelesInfo(accountBEDefinitionId, accountId, vrConnectionId);
        }
    }
}