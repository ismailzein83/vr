using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Web.Controllers
{
    public class ResetPasswordInput
    {
        public int UserId { get; set; }
        public string Password { get; set; }
    }

    public class UsersController : ApiController
    {
        [HttpGet]
        public IEnumerable<User> GetFilteredUsers(int fromRow, int toRow, string name, string email)
        {
            UserManager manager = new UserManager();
            return manager.GetFilteredUsers(fromRow, toRow, name, email);
        }

        [HttpGet]
        public IEnumerable<User> GetUsers()
        {
            UserManager manager = new UserManager();
            return manager.GetUsers();
        }

        [HttpGet]
        public List<User> GetMembers(int groupId)
        {
            UserManager manager = new UserManager();
            return manager.GetMembers(groupId);
        }

        [HttpGet]
        public User GetUserbyId(int userId)
        {
            UserManager manager = new UserManager();
            return manager.GetUserbyId(userId);
        }

        [HttpPost]
        public Vanrise.Entities.UpdateOperationOutput<User> UpdateUser(User userObject)
        {
            UserManager manager = new UserManager();
            return manager.UpdateUser(userObject);
        }

        [HttpPost]
        public Vanrise.Entities.InsertOperationOutput<User> AddUser(User userObject)
        {
            UserManager manager = new UserManager();
            return manager.AddUser(userObject);
        }

        [HttpGet]
        public bool CheckUserName(string name)
        {
            UserManager manager = new UserManager();
            return manager.CheckUserName(name);
        }

        [HttpPost]
        public bool ResetPassword(ResetPasswordInput user)
        {
            UserManager manager = new UserManager();
            return manager.ResetPassword(user.UserId, user.Password);
        }
    }
}