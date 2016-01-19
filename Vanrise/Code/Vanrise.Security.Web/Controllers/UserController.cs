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
    public class VR_Sec_UsersController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]      
        [Route("GetFilteredUsers")]
        [Authorization(Permissions = "Root/Administration Module/Users:View")]
        public object GetFilteredUsers(Vanrise.Entities.DataRetrievalInput<UserQuery> input)
        {
            UserManager manager = new UserManager();
            return GetWebResponse(input, manager.GetFilteredUsers(input));
        }

        [HttpGet]
        [Route("GetUsers")]
        public IEnumerable<UserInfo> GetUsers()
        {
            UserManager manager = new UserManager();
            return manager.GetUsers();
        }

        [HttpGet]
        [Authorization(Permissions = "Root/Administration Module/Users:View")]
        [Route("GetMembers")]
        public List<User> GetMembers(int groupId)
        {
            UserManager manager = new UserManager();
            return manager.GetMembers(groupId);
        }

        [HttpGet]
        [Authorization(Permissions = "Root/Administration Module/Users:View")]
        [Route("GetUserbyId")]
        public User GetUserbyId(int userId)
        {
            UserManager manager = new UserManager();
            return manager.GetUserbyId(userId);
        }

        [HttpPost]
        [Authorization(Permissions = "Root/Administration Module/Users:Edit")]
        [Route("UpdateUser")]
        public Vanrise.Entities.UpdateOperationOutput<UserDetail> UpdateUser(User userObject)
        {
            UserManager manager = new UserManager();
            return manager.UpdateUser(userObject);
        }

        [HttpPost]
        [Authorization(Permissions = "Root/Administration Module/Users:Add")]
        [Route("AddUser")]
        public Vanrise.Entities.InsertOperationOutput<UserDetail> AddUser(User userObject)
        {
            UserManager manager = new UserManager();
            return manager.AddUser(userObject);
        }

        [HttpGet]
        [Route("CheckUserName")]
        public bool CheckUserName(string name)
        {
            UserManager manager = new UserManager();
            return manager.CheckUserName(name);
        }

        [HttpPost]
        [Authorization(Permissions = "Root/Administration Module/Users:Reset Password")]
        [Route("ResetPassword")]
        public Vanrise.Entities.UpdateOperationOutput<object> ResetPassword(ResetPasswordInput user)
        {
            UserManager manager = new UserManager();
            return manager.ResetPassword(user.UserId, user.Password);
        }

        [HttpGet]
        [Route("LoadLoggedInUserProfile")]
        public UserProfile LoadLoggedInUserProfile()
        { 
         UserManager manager = new UserManager();
         return manager.LoadLoggedInUserProfile();
        }

        [HttpPost]
        [Route("EditUserProfile")]
        public Vanrise.Entities.UpdateOperationOutput<UserProfile> EditUserProfile(UserProfile userProfileObject)
        {
            UserManager manager = new UserManager();
            return manager.EditUserProfile(userProfileObject);
        
        }
    }

    public class ResetPasswordInput
    {
        public int UserId { get; set; }
        public string Password { get; set; }
    }
}