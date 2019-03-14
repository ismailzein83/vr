using System;
using Vanrise.Common.Business;

namespace SOM.ST.Business
{
    public class BSCSOnlineInterceptor : VRHttpConnectionCallInterceptor
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public override Guid ConfigId { get { return new Guid("461B7474-9B19-4B90-AEAB-63BA37245E53"); } }

        private string HeaderText = @"
                <soapenv:Header>
                    <wsse:Security soapenv:mustUnderstand=""1"" xmlns:wsse=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"" xmlns:wsu=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">
                        <wsse:UsernameToken wsu:Id=""UsernameToken-8FC6ED7B8E6B25B81515409786233051"">
                            <wsse:Username>##username##</wsse:Username>
                            <wsse:Password Type=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText"">##password##</wsse:Password>
                            <wsse:Nonce EncodingType=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary"">67ah3oBQJcOJ0QsYfJ+W9w==</wsse:Nonce>
                            <wsu:Created>2018-10-31T09:37:03.298Z</wsu:Created>
                        </wsse:UsernameToken>
                    </wsse:Security>
                </soapenv:Header>
                     ";

        public override void InterceptRequest(IVRHttpConnectionInterceptRequestContext context)
        {
            var headerText = HeaderText;
            headerText = headerText.Replace("##username##", UserName);
            headerText = headerText.Replace("##password##", Password);
            context.Body = context.Body.Replace("<soapenv:Header />", headerText);
        }

        public override void InterceptResponse(IVRHttpConnectionInterceptResponseContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}