﻿using System;
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

    public class UsersController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        public object GetFilteredUsers(Vanrise.Entities.DataRetrievalInput<UserQuery> input)
        {
            UserManager manager = new UserManager();
            return GetWebResponse(input, manager.GetFilteredUsers(input));
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
        public Vanrise.Entities.UpdateOperationOutput<User> ResetPassword(ResetPasswordInput user)
        {
            UserManager manager = new UserManager();
            return manager.ResetPassword(user.UserId, user.Password);
        }
    }
}