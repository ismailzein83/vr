using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.Business;
using TOne.Entities;

namespace TOne.Main.Web.Controllers
{
    public class ResetPasswordInput
    {
        public int UserId { get; set; }
        public string Password { get; set; }
    }

    public class UserController : ApiController
    {
        [HttpGet]
        public IEnumerable<User> GetFilteredUsers(int fromRow, int toRow, string name, string email)
        {
            UserManager manager = new UserManager();
            return ((IEnumerable<User>)(manager.GetFilteredUsers(fromRow, toRow, name, email)));
        }

        [HttpGet]
        public User GetUser(int userId)
        {
            UserManager manager = new UserManager();
            return ((User)(manager.GetUser(userId)));
        }

        [HttpPost]
        public TOne.Entities.UpdateOperationOutput<User> UpdateUser(User userObject)
        {
            UserManager manager = new UserManager();
            return manager.UpdateUser(userObject);
        }

        [HttpPost]
        public TOne.Entities.OperationResults.InsertOperationOutput<User> AddUser(User userObject)
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