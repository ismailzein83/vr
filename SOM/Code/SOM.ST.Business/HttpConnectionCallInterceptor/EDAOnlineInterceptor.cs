using System;
using Vanrise.Common.Business;

namespace SOM.ST.Business
{
    public class EDAOnlineInterceptor : VRHttpConnectionCallInterceptor
    {
        public string SessionId { get; set; }

        public override Guid ConfigId { get { return new Guid("461B7474-9B19-4B90-AEAB-63BA37245E53"); } }

        private string HeaderText = @"
                <soapenv:Header>
                    <cai3:SessionId  xmlns:cai3=""http://schemas.ericsson.com/cai3g1.2/"" >${##SessionId##}</cai3:SessionId>
                </soapenv:Header>";

        public override void InterceptRequest(IVRHttpConnectionInterceptRequestContext context)
        {
            var headerText = HeaderText;
            headerText = headerText.Replace("##SessionId##", SessionId);
            context.Body = context.Body.Replace("<soapenv:Header />", headerText);
        }

        public override void InterceptResponse(IVRHttpConnectionInterceptResponseContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}