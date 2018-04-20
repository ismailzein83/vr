using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;
using Terrasoft.Core.Configuration;
using System.Web;
using Terrasoft.Core;

namespace BPMExtended.Main.Common
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
            object email = "admin@vanrise.com";
            object password = "1";
            //SysSettings.TryGetValue(this.BPM_UserConnection, "SOM_USER_ID", out email);
            //SysSettings.TryGetValue(this.BPM_UserConnection, "SOM_LOGIN_PASSWORD", out password);

            CredentialsInput credentialsInput = new CredentialsInput() { Email = email as string, Password = password as string };
            var output = LocalWebAPIClient.Post<CredentialsInput, AuthenticateOperationOutput<AuthenticationToken>>(GetSOMBaseURL(),
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
            return LocalWebAPIClient.Post<T, Q>(GetSOMBaseURL(), actionPath, request, headers);
        }

        public T Get<T>(string actionPath)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            AddAuthenticationTokenToHeader(headers);
            return LocalWebAPIClient.Get<T>(GetSOMBaseURL(), actionPath, headers);
        }

        private string GetSOMBaseURL()
        {
        //    object somBaseURL;
        //    SysSettings.TryGetValue(this.BPM_UserConnection, "SOM_BASE_ADDRESS", out somBaseURL);
        //    return somBaseURL as string;
            return "http://192.168.25.9:8089";
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
