using Demo.Module.Business;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "User")]
    public class Demo_Module_UserController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredUsers")]
        public object GetFilteredUsers(Vanrise.Entities.DataRetrievalInput<UserQ> input)
        {
            UserManager manager = new UserManager();
            return GetWebResponse(input, manager.GetFilteredUsers(input));

        }

        [HttpPost]
        [Route("AddUser")]
        public Vanrise.Entities.InsertOperationOutput<UserDetails> AddUser(User user)
        {
            UserManager manager = new UserManager();
            return manager.AddUser(user);
        }

        [HttpGet]
        [Route("GetUser")]
        public User GetUser(int Id)
        {
            UserManager manager = new UserManager();
            return manager.GetUser(Id);
        }
        [HttpPost]
        [Route("UpdateUser")]
        public Vanrise.Entities.UpdateOperationOutput<UserDetails> UpdateUser(User user)
        {
            UserManager manager = new UserManager();
            return manager.UpdateUser(user);
        }

    }
}