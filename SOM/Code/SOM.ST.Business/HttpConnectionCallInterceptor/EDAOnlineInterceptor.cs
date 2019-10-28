using System;
using System.IO;
using System.Net;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace SOM.ST.Business
{
    public class EDAOnlineInterceptor : VRHttpConnectionCallInterceptor
    {

        public override Guid ConfigId { get { return new Guid("6D0083C3-E7C9-483F-B87C-5CAF4CD6F4D7"); } }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string AuthenticationServiceURI { get; set; }

        public override void InterceptRequest(IVRHttpConnectionInterceptRequestContext context)
        {
            var fullAuthenticationServiceURI = string.Concat(context.Connection.BaseURL, AuthenticationServiceURI);
            var sessionId = TryLogin(fullAuthenticationServiceURI);
            var headerText = HeaderText;
            headerText = headerText.Replace("##sessionId##", sessionId);
            context.Body = context.Body.Replace("<soapenv:Header />", headerText);
        }

        public override void InterceptResponse(IVRHttpConnectionInterceptResponseContext context)
        {
            throw new System.NotImplementedException();
        }

        private string TryLogin(string authURI)
        {
            var sessionId = string.Empty;
            var loginRequestBody = LoginRequestBody;
            loginRequestBody = loginRequestBody.Replace("##userId##", UserId);
            loginRequestBody = loginRequestBody.Replace("##password##", Password);

            var authRequest = HttpWebRequest.Create(authURI) as HttpWebRequest;
            authRequest.Method = "POST";
            authRequest.ContentType = "text/xml";
            using (var requesrStream = authRequest.GetRequestStream())
            {
                using (var writer = new StreamWriter(requesrStream))
                {
                    writer.Write(loginRequestBody);
                }
            }

            using (var response = (HttpWebResponse)authRequest.GetResponse())
            {
                var loginResponse = new VRXmlSerializer().Deserialize<LogInResponse>(response.ToString());
                sessionId = loginResponse.sessionId;
            }

            return sessionId;
        }

        private class LogInResponse
        {
            public string sessionId { get; set; }
        }

        private string LoginRequestBody = @"
                        <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:cai3=""http://schemas.ericsson.com/cai3g1.2/"">
                            <soapenv:Header/>
                            <soapenv:Body>
                                <cai3:Login>
                                <cai3:userId>##userId##</cai3:userId>
                                <cai3:pwd>##password##</cai3:pwd>
                                </cai3:Login>
                            </soapenv:Body>
                        </soapenv:Envelope>";

        private string HeaderText = @"
                                      <soapenv:Header>
                                          <cai3:SessionId>##sessionId##</cai3:SessionId>
                                      </soapenv:Header>
                                    ";
    }
}