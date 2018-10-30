using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace PartnerPortal.CustomerAccess.MainExtensions.VRRestAPIAnalyticQueryInterceptor
{
    public class WhSAccountVRRestAPIAnalyticQueryInterceptor : Vanrise.Analytic.Entities.VRRestAPIAnalyticQueryInterceptor
    {
        public override Guid ConfigId { get { return new Guid("2cf9eade-97ab-467f-b24b-35345324930a"); } }

        public override void PrepareQuery(IVRRestAPIAnalyticQueryInterceptorContext context)
        {
            throw new NotImplementedException();
        }
    }
}
