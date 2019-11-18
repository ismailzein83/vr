using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common.Business
{
    public class VRCookieAuthHttpConnectionCallInterceptor : VRHttpConnectionCallInterceptor
    {
        public override Guid ConfigId { get { return new Guid("E0E15373-0F94-45A2-BBEE-73ACEB3D3301"); } }
        public string Username { get; set; }
        public string Password { get; set; }

        public override void InterceptRequest(IVRHttpConnectionInterceptRequestContext context)
        {
            throw new NotImplementedException();
        }

        public override void InterceptResponse(IVRHttpConnectionInterceptResponseContext context)
        {
            throw new NotImplementedException();
        }
    }
}