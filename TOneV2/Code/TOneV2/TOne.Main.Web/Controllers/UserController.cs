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
    public class UserController : ApiController
    {
        [HttpGet]
        public List<User> GetUsers()
        {
            SecurityManager manager = new SecurityManager();
            return manager.GetUsers();
        }

        [HttpGet]
        public void DeleteUser(int Id)
        {
            SecurityManager manager = new SecurityManager();
            manager.DeleteUser(Id);
        }

        [HttpPost]
        public void SaveUser(User user)
        {
            SecurityManager manager = new SecurityManager();
            manager.SaveUser(user);
        }

        [HttpGet]
        public List<User> SearchUser(string Name, string Email)
        {
            SecurityManager manager = new SecurityManager();
            return manager.SearchUser(Name, Email);
        }
    }
}