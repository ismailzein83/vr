using PartnerPortal.CustomerAccess.Business;
using PartnerPortal.CustomerAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Security.Entities;
using Vanrise.Web.Base;

namespace PartnerPortal.CustomerAccess.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "WhSAccountUser")]
    [JSONWithTypeAttribute]
    public class WhSAccountUserController : BaseAPIController
    {
        //WhSAccountUserManager _manager = new WhSAccountUserManager();

        //[HttpPost]
        //[Route("AddWhSAccountUser")]
        //[VRSecurityActionAttribute("VR_Sec/Users/AddUser")]
        //public Vanrise.Entities.InsertOperationOutput<UserDetail> AddWhSAccountUser(WhSAccount whSAccount)
        //{
        //    return _manager.AddWhSAccountUser(whSAccount);
        //}
        //[HttpGet]
        //[Route("GetUserStatusByUserId")]
        //// [VRSecurityActionAttribute("VR_Sec/Users/GetUserStatusByUserId")]
        //public UserStatus GetUserStatusByUserId(int userId)
        //{
        //    return _manager.GetUserStatusByUserId(userId);
        //}
        //[HttpGet]
        //[Route("UnlockPortalAccount")]
        //// [VRSecurityActionAttribute("VR_Sec/Users/GetUserStatusByUserId")]
        //public UpdateOperationOutput<UserDetail> UnlockPortalAccount(int userId)
        //{
        //    return _manager.UnlockPortalAccount(userId);
        //}
        //[HttpPost]
        //[Route("UpdateWhSAccountUser")]
        ////[VRSecurityActionAttribute("VR_Sec/Users/UpdateUser")]
        //public Vanrise.Entities.UpdateOperationOutput<UserDetail> UpdateWhSAccountUser(WhSAccountToUpdate whSAccount)
        //{
        //    return _manager.UpdateWhSAccountUser(whSAccount);
        //}
        //[HttpPost]
        //[Route("GetUsersStatus")]
        //// [VRSecurityActionAttribute("VR_Sec/Users/GetUsersStatus")]
        //public List<UserDetailInfo> GetUsersStatus(UserStatusInput userStatusInput)
        //{
        //    return _manager.GetUsersStatus(userStatusInput);
        //}

        //[HttpPost]
        //[Route("ResetPassword")]
        //[VRSecurityActionAttribute("VR_Sec/Users/ResetPassword")]
        //public Vanrise.Entities.UpdateOperationOutput<object> ResetPassword(Vanrise.Security.Entities.ResetPasswordInput resetPasswordInput)
        //{
        //    return _manager.ResetPassword(resetPasswordInput.UserId, resetPasswordInput.Password);
        //}
        //[HttpGet]
        //[Route("EnableUser")]
        //[VRSecurityActionAttribute("VR_Sec/Users/EnableUser")]
        //public UpdateOperationOutput<UserDetail> EnableUser(int userId)
        //{
        //    return _manager.EnableUser(userId);
        //}
        //[HttpGet]
        //[Route("DisableUser")]
        //[VRSecurityActionAttribute("VR_Sec/Users/DisableUser")]
        //public UpdateOperationOutput<UserDetail> DisableUser(int userId)
        //{
        //    return _manager.DisableUser(userId);
        //}


    }
}