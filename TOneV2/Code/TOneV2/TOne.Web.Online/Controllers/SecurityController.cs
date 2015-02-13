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
                int expirationPeriodInMinutes = 15;
                SecurityToken userInfo = new SecurityToken
                {
                    UserId = output.User.ID,
                    Username = output.User.Login,
                    UserDisplayName = output.User.Name,
                    IssuedAt = DateTime.Now,
                    ExpiresAt = DateTime.Now.AddMinutes(expirationPeriodInMinutes)
                };
                string encrypted = EncryptionHelper.Encrypt(Serializer.Serialize(userInfo));
                AuthenticationToken returnedToken = new AuthenticationToken
                {
                    TokenName = "X-Token",
                    ExpirationIntervalInMinutes = expirationPeriodInMinutes,
                    Username = userInfo.Username,
                    UserDisplayName= userInfo.UserDisplayName,
                    Token = encrypted
                };
                return this.Request.CreateResponse<AuthenticationToken>(HttpStatusCode.OK, returnedToken);
            }
            else
            {
                throw new HttpResponseException(new HttpResponseMessage() { StatusCode = HttpStatusCode.Unauthorized, Content = new StringContent("Invalid user name or password.") });
            }
        }

       private class AuthenticationToken
       {
           public string TokenName { get; set; }

           public int ExpirationIntervalInMinutes { get; set; }

           public string Username { get; set; }

           public string UserDisplayName { get; set; }

           public string Token { get; set; }
       }
    }

    
}