using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Security.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "Users")]
    [ConstVRActionAuditAttribute("Security", "User")]
    public class UsersController : Vanrise.Web.Base.BaseAPIController
    {
        private UserManager _manager = new UserManager();

        public UsersController()
        {
            this._manager = new UserManager();
        }

        [HttpPost]
        [Route("GetFilteredUsers")]
        [ConstVRActionAuditAttribute(ConstVRActionAuditActionTypes.GETFILTERED)]
        public object GetFilteredUsers(Vanrise.Entities.DataRetrievalInput<UserQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredUsers(input));
        }

        [HttpGet]
        [Route("GetUsersInfo")]
        public IEnumerable<UserInfo> GetUsersInfo(string filter = null)
        {
            UserFilter deserializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<UserFilter>(filter) : null;
            return _manager.GetUsersInfo(deserializedFilter);
        }

        [HttpGet]
        [Route("GetMembers")]
        public IEnumerable<int> GetMembers(int groupId)
        {
            UserGroupManager userGroupManager = new UserGroupManager();
            return userGroupManager.GetMembers(groupId);
        }

        [HttpGet]
        [Route("GetUserbyId")]
        [ConstVRActionAudit(ConstVRActionAuditActionTypes.GETITEM, ObjectIdArgName = "userId")]
        public User GetUserbyId(int userId)
        {
            return _manager.GetUserbyId(userId);
        }

        [HttpPost]
        [Route("UpdateUser")]
        [ConstVRActionAudit(ConstVRActionAuditActionTypes.UPDATE, ObjectIdArgName = "userObject", ObjectIdPropPath = "UserId")]
        public Vanrise.Entities.UpdateOperationOutput<UserDetail> UpdateUser(User userObject)
        {
            return _manager.UpdateUser(userObject);
        }

        [HttpPost]
        [Route("DisableUser")]
        [ConstVRActionAudit(ConstVRActionAuditActionTypes.DISABLE, ObjectIdArgName = "userObject", ObjectIdPropPath = "UserId")]
        public Vanrise.Entities.UpdateOperationOutput<UserDetail> DisableUser(User userObject)
        {
            return _manager.DisableUser(userObject);
        }

        [HttpPost]
        [Route("EnableUser")]
        [ConstVRActionAudit(ConstVRActionAuditActionTypes.ENABLE, ObjectIdArgName = "userObject", ObjectIdPropPath = "UserId")]
        public Vanrise.Entities.UpdateOperationOutput<UserDetail> EnableUser(User userObject)
        {
            return _manager.EnableUser(userObject);
        }
        [HttpPost]
        [Route("AddUser")]
        [ConstVRActionAudit(ConstVRActionAuditActionTypes.ADD, ObjectIdArgName = "userObject", ObjectIdPropPath = "UserId")]
        public Vanrise.Entities.InsertOperationOutput<UserDetail> AddUser(User userObject)
        {
            return _manager.AddUser(userObject);
        }

        [HttpGet]
        [Route("CheckUserName")]
        public bool CheckUserName(string name)
        {
            return _manager.CheckUserName(name);
        }

        [HttpPost]
        [Route("ResetPassword")]
        [ConstVRActionAudit("Reset Password", ObjectIdArgName = "user", ObjectIdPropPath = "UserId")]
        public Vanrise.Entities.UpdateOperationOutput<object> ResetPassword(ResetPasswordInput user)
        {
            return _manager.ResetPassword(user.UserId, user.Password);
        }

        [IsAnonymous]
        [HttpPost]
        [Route("ActivatePassword")]
        public Vanrise.Entities.UpdateOperationOutput<object> ActivatePassword(ActivatePasswordInput user)
        {
            return _manager.ActivatePassword(user.Email, user.Password, user.Name, user.TempPassword);
        }


        [HttpGet]
        [Route("LoadLoggedInUserProfile")]
        public UserProfile LoadLoggedInUserProfile()
        {
            return _manager.LoadLoggedInUserProfile();
        }

        [HttpPost]
        [Route("EditUserProfile")]
        public Vanrise.Entities.UpdateOperationOutput<UserProfile> EditUserProfile(UserProfile userProfileObject)
        {
            return _manager.EditUserProfile(userProfileObject);

        }

        [IsAnonymous]
        [HttpPost]
        [Route("ForgotPassword")]
        public Vanrise.Entities.UpdateOperationOutput<object> ForgotPassword(ForgotPasswordInput forgotPasswordInput)
        {
            return _manager.ForgotPassword(forgotPasswordInput.Email);
        }
    }

    public class ForgotPasswordInput
    {
        public string Email { get; set; }
    }

    public class ActivatePasswordInput
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string TempPassword { get; set; }
    }
}