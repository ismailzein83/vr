using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Security.Entities;
using Terrasoft.Core.Configuration;
using System.Web;
using Terrasoft.Core;

namespace BPMExtended.Main.Business
{
    public class SOMClient : IDisposable
    {
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }

        AuthenticationToken _authToken;
        public SOMClient()
        {

            CredentialsInput credentialsInput = new CredentialsInput() { Email = "admin@vanrise.com", Password = "1" };
            var output = Vanrise.Common.VRWebAPIClient.Post<CredentialsInput, AuthenticateOperationOutput<AuthenticationToken>>(GetSOMBaseURL(),
                 "/api/VR_Sec/Security/Authenticate", credentialsInput, null);
            if (output.Result != AuthenticateOperationResult.Succeeded)
            {
                throw new Exception(String.Format("Authentication to SOM failed. Result: {0}. Message: {1}", output.Result.ToString(), output.Message));
            }
            _authToken = output.AuthenticationObject;
        }

        public Q Post<T, Q>(string actionPath, T request)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            AddAuthenticationTokenToHeader(headers);
            return VRWebAPIClient.Post<T, Q>(GetSOMBaseURL(), actionPath, request, headers, true);
        }

        public T Get<T>(string actionPath)
        {

            CustomerRequestManager customerRequestManager = new Business.CustomerRequestManager();
            customerRequestManager.CreateLineSubscriptionRequest(Entities.CustomerObjectType.Account, Guid.NewGuid(), new SOM.Main.BP.Arguments.LineSubscriptionRequest
            {
                CabinetId = "C1",
                DPId = "D1",
                PhoneNumber = "112233",
                SwitchId = "S1"

            });

            Dictionary<string, string> headers = new Dictionary<string, string>();
            AddAuthenticationTokenToHeader(headers);
            return VRWebAPIClient.Get<T>(GetSOMBaseURL(), actionPath, headers);
        }

        private string GetSOMBaseURL()
        {
            object somBaseURL;
            SysSettings.TryGetValue(this.BPM_UserConnection, "SOM_Base_Address", out somBaseURL);
            return somBaseURL as string;//"http://localhost:5559";
        }

        private void AddAuthenticationTokenToHeader(Dictionary<string, string> headers)
        {
            headers.Add(_authToken.TokenName, _authToken.Token);
        }

        public void Dispose()
        {

        }
    }
}
