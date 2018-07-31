using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Analytic.Business
{
    public class VRReportGenerationViewSettings:ViewSettings
    {
        public override string GetURL(View view)
        {
            return view.Url;
        }

        public override bool DoesUserHaveAccess(IViewUserAccessContext context)
        {
            /// return true if it has permission on at least one query 
            return true;
        }
    }
}
