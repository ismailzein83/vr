﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Web.Controllers
{
    public class SecurityController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        public AuthenticationToken Authenticate(CredentialsInput credentialsObject)
        {
            SecurityManager manager = new SecurityManager();
            return manager.Authenticate(credentialsObject.Email, credentialsObject.Password);
        }

         [HttpGet]
        public Vanrise.Entities.UpdateOperationOutput<object> ResetPassword(string oldPassword,string newPassword)
        {
            int loggedInUserId = SecurityContext.Current.GetLoggedInUserId();
            SecurityManager manager = new SecurityManager();
            return manager.ResetPassword(loggedInUserId,oldPassword ,newPassword);
        }

        public class CredentialsInput
        {
            public string Email { get; set; }

            public string Password { get; set; }
        }

    }
}