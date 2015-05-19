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
        public IEnumerable<User> GetUsers(int pageNumber, int pageSize)
        {
            SecurityManager manager = new SecurityManager();
            return ((IEnumerable<User>)(manager.GetUsers())).Skip(pageNumber * pageSize).Take(pageSize);
        }

        [HttpGet]
        public void DeleteUser(int Id)
        {
            SecurityManager manager = new SecurityManager();
            manager.DeleteUser(Id);
        }

        [HttpPost]
        public void EditUser(User user)
        {
            SecurityManager manager = new SecurityManager();
            manager.EditUser(user);
        }

        public void AddUser(User user)
        {
            SecurityManager manager = new SecurityManager();
            manager.AddUser(user);
        }


        [HttpGet]
        public List<User> SearchUser(string Name, string Email)
        {
            SecurityManager manager = new SecurityManager();
            return manager.SearchUser(Name, Email);
        }
    }
}