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
    public class UsersController : Vanrise.Web.Base.BaseAPIController
    {
        private UserManager _manager = new UserManager();

        public UsersController()
        {
            this._manager = new UserManager();
        }

        [HttpPost]
        [Route("GetFilteredUsers")]
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
        [Route("GetUserHistoryDetailbyHistoryId")]
        public User GetUserHistoryDetailbyHistoryId(int userHistoryId)
        {
            return _manager.GetUserHistoryDetailbyHistoryId(userHistoryId);
        }
        [HttpGet]
        [Route("GetLoggedInUserLanguageId")]
        public Guid? GetLoggedInUserLanguageId()
        {
            return _manager.GetLoggedInUserLanguageId();
        }

        [HttpGet]
        [Route("GetUserbyId")]
        public User GetUserbyId(int userId)
        {
            return _manager.GetUserbyId(userId, true);
        }

        [HttpPost]
        [Route("UpdateUser")]
        public Vanrise.Entities.UpdateOperationOutput<UserDetail> UpdateUser(UserToUpdate userObject)
        {
            return _manager.UpdateUser(userObject);
        }

        [HttpPost]
        [Route("UpdateUserExpiration")]
        public Vanrise.Entities.UpdateOperationOutput<UserDetail> UpdateUserExpiration(UserExpirationInput userExpirationInput)
        {
            return _manager.UpdateUserExpiration(userExpirationInput.UserId, userExpirationInput.EnabledTill);
        }

        [HttpPost]
        [Route("UpdateLoggedInUserLanguage")]
        public UpdateOperationOutput<Guid?> UpdateLoggedInUserLanguage(UpdateLanguageInput languageInput)
        {
            return _manager.UpdateLoggedInUserLanguage(languageInput.LanguageId);
        }

        [HttpPost]
        [Route("DisableUser")]
        public Vanrise.Entities.UpdateOperationOutput<UserDetail> DisableUser(UserDisableInput userObject)
        {
            return _manager.DisableUser(userObject.UserId);
        }

        [HttpPost]
        [Route("EnableUser")]
        public Vanrise.Entities.UpdateOperationOutput<UserDetail> EnableUser(UserEnableInput userObject)
        {
            return _manager.EnableUser(userObject.UserId);
        }
        [HttpPost]
        [Route("UnlockUser")]
        public Vanrise.Entities.UpdateOperationOutput<UserDetail> UnlockUser(User userObject)
        {
            return _manager.UnlockUser(userObject);
        }
        [HttpPost]
        [Route("AddUser")]
        public Vanrise.Entities.InsertOperationOutput<UserDetail> AddUser(UserToAdd userObject)
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

        [HttpGet]
        [Route("GetUserDetailsByEmail")]
        public UserDTO GetUserDetailsByEmail(string email)
        {
            return _manager.GetUserDetailsByEmail(email);
        }

        [HttpGet]
        [Route("GetUserDetailsById")]
        public UserDTO GetUserDetailsById(int userId)
        {
            return _manager.GetUserDetailsById(userId);
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
    public class UpdateLanguageInput
    {
        public Guid LanguageId { get; set; }
    }

    public class UserExpirationInput
    {
        public int UserId { get; set; }

        public DateTime? EnabledTill { get; set; }

    }

    public class UserEnableInput
    {
        public int UserId { get; set; }

    }

    public class UserDisableInput
    {
        public int UserId { get; set; }
    }
}
