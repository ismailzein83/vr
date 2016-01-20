using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
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
        [Authorization(Permissions = "Root/Administration Module/Users:View")]
        public object GetFilteredUsers(Vanrise.Entities.DataRetrievalInput<UserQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredUsers(input));
        }

        [HttpGet]
        [Route("GetUsers")]
        public IEnumerable<UserInfo> GetUsers()
        {
            return _manager.GetUsers();
        }

        [HttpGet]
        [Authorization(Permissions = "Root/Administration Module/Users:View")]
        [Route("GetMembers")]
        public IEnumerable<int> GetMembers(int groupId)
        {
            UserGroupManager userGroupManager = new UserGroupManager();
            return userGroupManager.GetMembers(groupId);
        }

        [HttpGet]
        [Authorization(Permissions = "Root/Administration Module/Users:View")]
        [Route("GetUserbyId")]
        public User GetUserbyId(int userId)
        {
            return _manager.GetUserbyId(userId);
        }

        [HttpPost]
        [Authorization(Permissions = "Root/Administration Module/Users:Edit")]
        [Route("UpdateUser")]
        public Vanrise.Entities.UpdateOperationOutput<UserDetail> UpdateUser(User userObject)
        {
            return _manager.UpdateUser(userObject);
        }

        [HttpPost]
        [Authorization(Permissions = "Root/Administration Module/Users:Add")]
        [Route("AddUser")]
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
        [Authorization(Permissions = "Root/Administration Module/Users:Reset Password")]
        [Route("ResetPassword")]
        public Vanrise.Entities.UpdateOperationOutput<object> ResetPassword(ResetPasswordInput user)
        {
            return _manager.ResetPassword(user.UserId, user.Password);
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
    }

    public class ResetPasswordInput
    {
        public int UserId { get; set; }
        public string Password { get; set; }
    }
}