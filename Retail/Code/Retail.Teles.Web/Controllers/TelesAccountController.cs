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
    public class TelesAccountController : BaseAPIController
    {
        TelesAccountManager _manager = new TelesAccountManager();

        [HttpPost]
        [Route("UnmapTelesAccount")]
        public object UnmapTelesAccount(TelesAccountToUnmap input)
        {
            if (!_manager.DoesUserHaveExecutePermission(input.AccountBEDefinitionId))
                return GetUnauthorizedResponse();
            return _manager.UnmapTelesAccount(input);
        }

    }
}