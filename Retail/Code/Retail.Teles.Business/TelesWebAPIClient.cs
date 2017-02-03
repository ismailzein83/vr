using Retail.BusinessEntity.Business;
using Retail.Teles.Business.SwitchIntegrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Retail.Teles.Business
{
    public class TelesWebAPIClient
    {
        public static Q Post<T, Q>(int switchId, string actionPath, T request)
        {
            TelesAPISwitchIntegration telesAPISwitch = GetTelesAPISwitchIntegration(switchId);
            var headers = GetHeaders(telesAPISwitch.Token, telesAPISwitch.Authorization);
            var finalActionPath = GetURL(telesAPISwitch.ActionPrefix, actionPath);
            return VRWebAPIClient.Post<T, Q>(telesAPISwitch.URL, finalActionPath, request, headers);
        }
        public static T Get<T>(int switchId, string actionPath)
        {
            TelesAPISwitchIntegration telesAPISwitch = GetTelesAPISwitchIntegration(switchId);
            var headers = GetHeaders(telesAPISwitch.Token, telesAPISwitch.Authorization);
            var finalActionPath = GetURL(telesAPISwitch.ActionPrefix, actionPath);
            return VRWebAPIClient.Get<T>(telesAPISwitch.URL, finalActionPath, headers);
        }
        public static Q Put<T, Q>(int switchId, string actionPath, T request)
        {
            TelesAPISwitchIntegration telesAPISwitch = GetTelesAPISwitchIntegration(switchId);
            var headers = GetHeaders(telesAPISwitch.Token, telesAPISwitch.Authorization);
            var finalActionPath = GetURL(telesAPISwitch.ActionPrefix, actionPath);
            return VRWebAPIClient.Put<T, Q>(telesAPISwitch.URL, finalActionPath, request, headers);
        }
        static TelesAPISwitchIntegration GetTelesAPISwitchIntegration(int switchId)
        {
            SwitchManager switchManager = new SwitchManager();
            var switchEntity = switchManager.GetSwitch(switchId);
            return switchEntity.Settings.SwitchIntegration as TelesAPISwitchIntegration;
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
