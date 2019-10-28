using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace SOM.ST.Business
{
    public class BPMOnlineInterceptor : VRHttpConnectionCallInterceptor
    {
        public override Guid ConfigId { get { return new Guid("2A61A473-A949-4781-8D06-DCB170006A3B"); } }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string AuthenticationServiceURI { get; set; }

        public override void InterceptRequest(IVRHttpConnectionInterceptRequestContext context)
        {
            var userId = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();

            var userManager = new Vanrise.Security.Business.UserManager();
            var user = userManager.GetUserbyId(userId);
            user.ThrowIfNull("LoggedInUser");
            var userEmail = user.Email;
            var somDevMode_BPMConfiguration = ConfigurationManager.AppSettings["SOMDevMode_BPMConfiguration"];

            if (!String.IsNullOrEmpty(somDevMode_BPMConfiguration))
            {
                var bpmConfigurations = somDevMode_BPMConfiguration.Split('|');
                foreach (var item in bpmConfigurations)
                {
                    var items = item.Split(';');
                    if (item.Length > 0)
                    {
                        if (items[0] == userEmail)
                        {
                            if (string.IsNullOrEmpty(items[1]))
                                throw new NullReferenceException(string.Format("User '{0}' does not have email in AppSettings", user.Name));
                            context.Connection.BaseURL = items[1];
                            context.Client.BaseAddress = new Uri(items[1]);
                            break;
                        }
                    }
                }
            }

            var fullAuthenticationServiceURI = string.Concat(context.Connection.BaseURL, AuthenticationServiceURI);
            var cookiesContainer = TryLogin(UserName, Password, fullAuthenticationServiceURI);
            cookiesContainer.ThrowIfNull("cookiesContainer");

            string cookiesAsString = GetCookiesAsString(fullAuthenticationServiceURI, cookiesContainer);
            var cookies = cookiesContainer.GetCookies(new Uri(fullAuthenticationServiceURI));

            foreach (Cookie cookie in cookies)
            {
                if (cookie.Name == "BPMCSRF")
                {
                    context.HttpRequestMessage.Headers.Add(cookie.Name, cookie.Value);
                    context.Client.DefaultRequestHeaders.Add(cookie.Name, cookie.Value);
                }
            }
            context.HttpRequestMessage.Headers.Add("Cookie", cookiesAsString);
            context.Client.DefaultRequestHeaders.Add("Cookie", cookiesAsString);
        }

        public override void InterceptResponse(IVRHttpConnectionInterceptResponseContext context)
        {
            throw new System.NotImplementedException();
        }

        #region PrivateMethods
        private string GetCookiesAsString(string fullAuthenticationServiceURI, CookieContainer cookiesContainer)
        {
            var cookies = cookiesContainer.GetCookies(new Uri(fullAuthenticationServiceURI));

            List<string> cookiesAsString = new List<string>();

            foreach (Cookie cookie in cookies)
            {
                var cookieAsString = string.Format("{0}={1}", cookie.Name, cookie.Value);
                cookiesAsString.Add(cookieAsString);
            }

            return string.Join("; ", cookiesAsString);
        }

        private CookieContainer TryLogin(string userName, string userPassword, string authURI)
        {
            var authRequest = HttpWebRequest.Create(authURI) as HttpWebRequest;
            authRequest.Method = "POST";
            authRequest.ContentType = "application/json";
            CookieContainer AuthCookie = new CookieContainer();
            authRequest.CookieContainer = AuthCookie;
            using (var requesrStream = authRequest.GetRequestStream())
            {
                using (var writer = new StreamWriter(requesrStream))
                {
                    writer.Write(@"{
                        ""UserName"":""" + userName + @""",
                        ""UserPassword"":""" + userPassword + @"""
                    }");
                }
            }

            using (var response = (HttpWebResponse)authRequest.GetResponse())
            {
                return AuthCookie;
            }
        }
        #endregion
    }
}