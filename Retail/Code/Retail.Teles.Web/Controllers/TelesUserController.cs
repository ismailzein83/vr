﻿using Retail.BusinessEntity.Entities;
using Retail.Teles.Business;
using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
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
        [HttpGet]
        [Route("GetUserTelesInfo")]
        public TelesUserMappingInfo GetUserTelesInfo(Guid accountBEDefinitionId, long accountId, Guid vrConnectionId)
        {
            return _manager.GetUserTelesInfo(accountBEDefinitionId, accountId, vrConnectionId);
        }
        [HttpGet]
        [Route("GetUserTelesSiteId")]
        public string GetUserTelesSiteId(Guid accountBEDefinitionId, long accountId, Guid vrConnectionId)
        {
            return _manager.GetUserTelesSiteId(accountBEDefinitionId, accountId, vrConnectionId);
        }
        [HttpGet]
        [Route("GetCurrentUserRoutingGroupId")]
        public string GetCurrentUserRoutingGroupId(Guid accountBEDefinitionId, long accountId, Guid vrConnectionId)
        {
            return _manager.GetCurrentUserRoutingGroupId(accountBEDefinitionId, accountId, vrConnectionId);
        }
        [HttpGet]
        [Route("ChangeUserRoutingGroup")]
        public UpdateOperationOutput<AccountDetail> ChangeUserRoutingGroup(Guid accountBEDefinitionId, long accountId, Guid vrConnectionId, string routingGroupId)
        {
            return _manager.ChangeUserRoutingGroup(accountBEDefinitionId, accountId, vrConnectionId, routingGroupId);
        }
    }
}