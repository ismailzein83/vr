using System;
using System.Collections.Generic;
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
            var fullAuthenticationServiceURI = string.Concat(context.Connection.BaseURL, AuthenticationServiceURI);
            var cookiesContainer = TryLogin(UserName, Password, fullAuthenticationServiceURI);
            cookiesContainer.ThrowIfNull("cookiesContainer");

            string cookiesAsString = GetCookiesAsString(fullAuthenticationServiceURI, cookiesContainer);
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