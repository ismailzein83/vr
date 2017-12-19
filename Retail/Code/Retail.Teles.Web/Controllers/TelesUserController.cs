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
    [RoutePrefix(Constants.ROUTE_PREFIX + "TelesUser")]
    public class TelesUserController : BaseAPIController
    {
        TelesUserManager _manager = new TelesUserManager();

        [HttpGet]
        [Route("GetUsersInfo")]
        public IEnumerable<TelesUserInfo> GetUsersInfo(Guid vrConnectionId, string siteId, string serializedFilter = null)
        {
            TelesUserFilter filter = Vanrise.Common.Serializer.Deserialize<TelesUserFilter>(serializedFilter);
            return _manager.GetUsersInfo(vrConnectionId,siteId, filter);
        }
        [HttpPost]
        [Route("MapUserToAccount")]
        public object MapUserToAccount(MapUserToAccountInput input)
        {
            if (!_manager.DoesUserHaveExecutePermission(input.AccountBEDefinitionId))
                return GetUnauthorizedResponse();
            return _manager.MapUserToAccount(input);
        }
        [HttpGet]
        [Route("GetAccountDIDsCount")]
        public int GetAccountDIDsCount(Guid accountBEDefinitionId, long accountId)
        {
            return _manager.GetAccountDIDsCount(accountBEDefinitionId, accountId);
        }
    }
}