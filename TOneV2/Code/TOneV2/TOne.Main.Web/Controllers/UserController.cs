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
        public IEnumerable<User> GetUsers(int fromRow, int toRow)
        {
            UserManager manager = new UserManager();
            //return ((IEnumerable<User>)(manager.GetUsers())).Skip(pageNumber * pageSize).Take(pageSize);
            return ((IEnumerable<User>)(manager.GetUsers(fromRow, toRow)));
        }

        [HttpGet]
        public void DeleteUser(int Id)
        {
            UserManager manager = new UserManager();
            manager.DeleteUser(Id);
        }

        [HttpPost]
        public bool UpdateUser(User user)
        {
            UserManager manager = new UserManager();
            return manager.UpdateUser(user);
        }

        public User AddUser(User user)
        {
            UserManager manager = new UserManager();
            return manager.AddUser(user);
        }


        [HttpGet]
        public List<User> SearchUser(string Name, string Email)
        {
            UserManager manager = new UserManager();
            return manager.SearchUser(Name, Email);
        }
    }
}