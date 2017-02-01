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
        public Q Post<T, Q>(int switchId, string actionPath, T request)
        {
            SwitchManager switchManager = new SwitchManager();
            var switchEntity = switchManager.GetSwitch(switchId);
            var telesAPISwitch = switchEntity.Settings.SwitchIntegration as TelesAPISwitchIntegration;
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("X-C5-Application", telesAPISwitch.Token);
            headers.Add(HttpRequestHeader.Authorization.ToString(), telesAPISwitch.Authorization);
            return VRWebAPIClient.Post<T, Q>(telesAPISwitch.URL, actionPath, request, headers);
        }
        public T Get<T>(int switchId, string actionPath)
        {
            SwitchManager switchManager = new SwitchManager();
            var switchEntity = switchManager.GetSwitch(switchId);
            var telesAPISwitch = switchEntity.Settings.SwitchIntegration as TelesAPISwitchIntegration;
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("X-C5-Application", telesAPISwitch.Token);
            headers.Add(HttpRequestHeader.Authorization.ToString(), telesAPISwitch.Authorization);
            return VRWebAPIClient.Get<T>(telesAPISwitch.URL, actionPath, headers);
        }

    }
}
