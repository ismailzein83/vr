using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.Business;
using TOne.Entities;
using TOne.Web.Online.Models;
using TOne.Web.Online.Security;
using Vanrise.Common;

namespace TOne.Web.Online.Controllers
{
    public class SecurityController : ApiController
    {
       [HttpGet]
        public HttpResponseMessage Authenticate(string username, string password)
        {
            if (String.IsNullOrWhiteSpace (username) || String.IsNullOrWhiteSpace(password))
                throw new HttpResponseException(new HttpResponseMessage() { StatusCode = HttpStatusCode.Unauthorized, Content = new StringContent("Please provide the credentials.") });

           SecurityManager manager = new SecurityManager();
           AuthenticationOutput output = manager.Authenticate(username, password);
            if (output.Result == AuthenticationResult.Succeeded)
            {
                SecurityToken userInfo = new SecurityToken
                {
                    UserId = output.User.UserId,
                    Username = output.User.Username
                };
                string encrypted = EncryptionHelper.Encrypt(Serializer.Serialize(userInfo));
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(encrypted) };
            }
            else
            {
                throw new HttpResponseException(new HttpResponseMessage() { StatusCode = HttpStatusCode.Unauthorized, Content = new StringContent("Invalid user name or password.") });
            }
        }
    }
}