using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Web.Controllers
{
    public class SecurityController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        public AuthenticateOperationOutput<AuthenticationToken> Authenticate(CredentialsInput credentialsObject)
        {
            SecurityManager manager = new SecurityManager();
            return manager.Authenticate(credentialsObject.Email, credentialsObject.Password);
        }

         [HttpPost]
        public Vanrise.Entities.UpdateOperationOutput<object> ChangePassword(ChangedPasswordObject changedPasswordObject)
        {          
            SecurityManager manager = new SecurityManager();
            return manager.ChangePassword(changedPasswordObject.OldPassword,changedPasswordObject.NewPassword);
        }

        public class CredentialsInput
        {
            public string Email { get; set; }

            public string Password { get; set; }
        }

        public class ChangedPasswordObject
        {
            public string OldPassword { get; set;}

            public string NewPassword {get; set;}
        }

    }
}