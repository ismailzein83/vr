using PartnerPortal.CustomerAccess.Business;
using PartnerPortal.CustomerAccess.Entities;
using Retail.BusinessEntity.APIEntities;
using System;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Entities;
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
        [HttpGet]
        [Route("GetUserStatusByUserId")]
       // [VRSecurityActionAttribute("VR_Sec/Users/GetUserStatusByUserId")]
        public UserStatus GetUserStatusByUserId(int userId)
        {
            RetailAccountUserManager _manager = new RetailAccountUserManager();
            return _manager.GetUserStatusByUserId(userId);
        }
        [HttpGet]
        [Route("UnlockPortalAccount")]
        // [VRSecurityActionAttribute("VR_Sec/Users/GetUserStatusByUserId")]
        public UpdateOperationOutput<UserDetail> UnlockPortalAccount(int userId)
        {
            RetailAccountUserManager _manager = new RetailAccountUserManager();
            return _manager.UnlockPortalAccount(userId);
        }
        [HttpPost]
        [Route("UpdateRetailAccountUser")]
        //[VRSecurityActionAttribute("VR_Sec/Users/UpdateUser")]
        public Vanrise.Entities.UpdateOperationOutput<UserDetail> UpdateRetailAccountUser(RetailAccountToUpdate retailAccount)
        {
            RetailAccountUserManager _manager = new RetailAccountUserManager();
            return _manager.UpdateRetailAccountUser(retailAccount);
        }
        [HttpPost]
        [Route("GetUsersStatus")]
       // [VRSecurityActionAttribute("VR_Sec/Users/GetUsersStatus")]
        public List<UserDetailInfo> GetUsersStatus(UserStatusInput userStatusInput)
        {
            RetailAccountUserManager _manager = new RetailAccountUserManager();
            return _manager.GetUsersStatus(userStatusInput);
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
        [Route("EnableUser")]
        [VRSecurityActionAttribute("VR_Sec/Users/EnableUser")]
        public UpdateOperationOutput<UserDetail> EnableUser(int userId)
        {
            RetailAccountUserManager _manager = new RetailAccountUserManager();
            return _manager.EnableUser(userId);
        }
        [HttpGet]
        [Route("DisableUser")]
        [VRSecurityActionAttribute("VR_Sec/Users/DisableUser")]
        public UpdateOperationOutput<UserDetail> DisableUser(int userId)
        {
            RetailAccountUserManager _manager = new RetailAccountUserManager();
            return _manager.DisableUser(userId);
        }


    }
}