﻿using PartnerPortal.CustomerAccess.Business;
using PartnerPortal.CustomerAccess.Entities;
using Retail.BusinessEntity.APIEntities;
using System;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Security.Entities;
using Vanrise.Web.Base;

namespace PartnerPortal.CustomerAccess.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "RetailAccountUser")]
    [JSONWithTypeAttribute]
    public class RetailAccountUserController : BaseAPIController
    {
        [HttpPost]
        [Route("AddRetailAccountUser")]
        [VRSecurityActionAttribute("VR_Sec/Users/AddUser")]
        public Vanrise.Entities.InsertOperationOutput<UserDetail> AddRetailAccountUser(RetailAccount retailAccount)
        {
            RetailAccountUserManager _manager = new RetailAccountUserManager();
            return _manager.AddRetailAccountUser(retailAccount);
        }

        [HttpPost]
        [Route("ResetPassword")]
        [VRSecurityActionAttribute("VR_Sec/Users/ResetPassword")]
        public Vanrise.Entities.UpdateOperationOutput<object> ResetPassword(Vanrise.Security.Entities.ResetPasswordInput resetPasswordInput)
        {
            RetailAccountUserManager _manager = new RetailAccountUserManager();
            return _manager.ResetPassword(resetPasswordInput.UserId, resetPasswordInput.Password);
        }

        [HttpGet]
        [Route("GetClientChildAccountsInfo")]
        public IEnumerable<ClientChildAccountInfo> GetClientChildAccountsInfo(Guid businessEntityDefinitionId)
        {
            RetailAccountUserManager _manager = new RetailAccountUserManager();
            return _manager.GetClientChildAccountsInfo(businessEntityDefinitionId);
        }
    }
}