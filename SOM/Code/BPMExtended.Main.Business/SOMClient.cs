using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Security.Entities;

namespace BPMExtended.Main.Business
{
    public class SOMClient : IDisposable
    {
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

        public Q Post<T,Q>(string actionPath, T request)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            AddAuthenticationTokenToHeader(headers);
            return VRWebAPIClient.Post<T, Q>(GetSOMBaseURL(), actionPath, request, headers, true);
        }

        public T Get<T>(string actionPath)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            AddAuthenticationTokenToHeader(headers);
            return VRWebAPIClient.Get<T>(GetSOMBaseURL(), actionPath, headers);
        }

        private string GetSOMBaseURL()
        {
            return "http://localhost:5559";
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
