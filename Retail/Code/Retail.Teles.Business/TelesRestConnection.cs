using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;

namespace Retail.Teles.Business
{
    public class TelesRestConnection: VRConnectionSettings
    {
        public override Guid ConfigId { get { return new Guid("A3D7F334-21A0-4875-8EA2-ED54B33F292C"); } }
        public string Token { get; set; }
        public string Authorization { get; set; }
        public string URL { get; set; }
        public string ActionPrefix { get; set; }
        public int DefaultDomainId { get; set; }

        public Q Post<T, Q>(string actionPath, T request)
        {
            var headers = GetHeaders(this.Token, this.Authorization);
            var finalActionPath = GetURL(this.ActionPrefix, actionPath);
            return VRWebAPIClient.Post<T, Q>(this.URL, finalActionPath, request, headers);
        }
        public T Get<T>(string actionPath)
        {
            var headers = GetHeaders(this.Token, this.Authorization);
            var finalActionPath = GetURL(this.ActionPrefix, actionPath);
            return VRWebAPIClient.Get<T>(this.URL, finalActionPath, headers);
        }
        public Q Put<T, Q>(string actionPath, T request)
        {
            var headers = GetHeaders(this.Token, this.Authorization);
            var finalActionPath = GetURL(this.ActionPrefix, actionPath);
            return VRWebAPIClient.Put<T, Q>(this.URL, finalActionPath, request, headers);
        }

        static Dictionary<string, string> GetHeaders(string token, string authorization)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("X-C5-Application", token);
            headers.Add(HttpRequestHeader.Authorization.ToString(), authorization);
            return headers;
        }
        static string GetURL(string actionPrefix, string actionPath)
        {
            return actionPrefix != null ? string.Format("{0}{1}", actionPrefix, actionPath) : actionPath;
        }


    }
}
