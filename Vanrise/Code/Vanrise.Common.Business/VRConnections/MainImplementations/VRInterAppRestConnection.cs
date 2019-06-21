using System;
using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.Security.Entities;

namespace Vanrise.Common.Business
{
    public class VRInterAppRestConnection : VRHttpConnection
    {
        public override Guid ConfigId { get { return new Guid("5CD2AAC3-1C74-401F-8010-8B9B5FD9C011"); } }
        
        public string Username { get; set; }

        public string Password { get; set; }

        public T Get<T>(string actionName)
        {
            CredentialsInput credentialsInput = new CredentialsInput() { Email = this.Username, Password = this.Password };
            return Get<T>(credentialsInput, actionName);
        }

        public Q Post<T, Q>(string actionName, T request, bool serializeWithFullType = false)
        {
            CredentialsInput credentialsInput = new CredentialsInput() { Email = this.Username, Password = this.Password };
            return Post<T, Q>(credentialsInput, actionName, request, serializeWithFullType);
        }

        public Q Put<T, Q>(string actionName, T request)
        {
            CredentialsInput credentialsInput = new CredentialsInput() { Email = this.Username, Password = this.Password };
            return Put<T, Q>(credentialsInput, actionName, request);
        }

        public T Get<T>(CredentialsInput credentialsInput, string actionName)
        {
            var result = Vanrise.Common.VRWebAPIClient.Post<CredentialsInput, AuthenticateOperationOutput<AuthenticationToken>>(BaseURL,
                 "/api/VR_Sec/Security/Authenticate", credentialsInput, null);
            if (result.Result != AuthenticateOperationResult.Succeeded)
                throw new Exception($"Failed: {result.Message}");
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add(result.AuthenticationObject.TokenName, result.AuthenticationObject.Token);

            return Vanrise.Common.VRWebAPIClient.Get<T>(BaseURL, actionName, headers);
        }

        public Q Post<T, Q>(CredentialsInput credentialsInput, string actionName, T request, bool serializeWithFullType = false)
        {
            var result = Vanrise.Common.VRWebAPIClient.Post<CredentialsInput, AuthenticateOperationOutput<AuthenticationToken>>(BaseURL,
                 "/api/VR_Sec/Security/Authenticate", credentialsInput, null);
            if (result.Result != AuthenticateOperationResult.Succeeded)
                throw new Exception($"Failed: {result.Message}");

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add(result.AuthenticationObject.TokenName, result.AuthenticationObject.Token);

            return Vanrise.Common.VRWebAPIClient.Post<T, Q>(BaseURL, actionName, request, headers, serializeWithFullType);
        }
        public Q Put<T, Q>(CredentialsInput credentialsInput, string actionName, T request)
        {
            var result = Vanrise.Common.VRWebAPIClient.Post<CredentialsInput, AuthenticateOperationOutput<AuthenticationToken>>(BaseURL,
                 "/api/VR_Sec/Security/Authenticate", credentialsInput, null);
            if (result.Result != AuthenticateOperationResult.Succeeded)
                throw new Exception($"Failed: {result.Message}");
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add(result.AuthenticationObject.TokenName, result.AuthenticationObject.Token);

            return Vanrise.Common.VRWebAPIClient.Put<T, Q>(BaseURL, actionName, request, headers);
        }
        public T AnonymousGet<T>(string actionName)
        {
            return Vanrise.Common.VRWebAPIClient.Get<T>(BaseURL, actionName);
        }

        public Q AnonymousPost<T, Q>(string actionName, T request, bool serializeWithFullType = false)
        {
            return Vanrise.Common.VRWebAPIClient.Post<T, Q>(BaseURL, actionName, request, null, serializeWithFullType);
        }
        
        public override VRHttpConnectionCallInterceptor Interceptor
        {
            get
            {
                return new VRInterAppRestConnectionHttpConnectionCallInterceptor(this.BaseURL, this.Username, this.Password);
            }
            set
            {
            }
        }

        #region Private Classes

        private class VRInterAppRestConnectionHttpConnectionCallInterceptor : VRHttpConnectionCallInterceptor
        {
            private string _baseURL;

            private string _username;

            private string _password;

            public override Guid ConfigId => new Guid();

            public VRInterAppRestConnectionHttpConnectionCallInterceptor(string baseURL, string username, string password)
            {
                this._baseURL = baseURL;
                this._username = username;
                this._password = password;
            }

            public override void InterceptRequest(IVRHttpConnectionInterceptRequestContext context)
            {
                CredentialsInput credentialsInput = new CredentialsInput() { Email = this._username, Password = this._password };
                var result = Vanrise.Common.VRWebAPIClient.Post<CredentialsInput, AuthenticateOperationOutput<AuthenticationToken>>(this._baseURL,
                 "/api/VR_Sec/Security/Authenticate", credentialsInput, null);
                if (result.Result != AuthenticateOperationResult.Succeeded)
                    throw new Exception($"Authentication Failed: {result.Message}");
                Dictionary<string, string> headers = new Dictionary<string, string>();
                context.HttpRequestMessage.Headers.Add(result.AuthenticationObject.TokenName, result.AuthenticationObject.Token);
            }

            public override void InterceptResponse(IVRHttpConnectionInterceptResponseContext context)
            {

            }
        }

        #endregion
    }

    public class VRInterAppRestConnectionFilter : IVRConnectionFilter
    {
        public Guid ConfigId { get { return new Guid("5CD2AAC3-1C74-401F-8010-8B9B5FD9C011"); } }

        public bool IsMatched(VRConnection vrConnection)
        {
            if (vrConnection == null)
                throw new NullReferenceException("connection");

            if (vrConnection.Settings == null)
                throw new NullReferenceException("vrConnection.Settings");

            if (vrConnection.Settings.ConfigId != ConfigId)
                return false;

            return true;
        }
    }
}
