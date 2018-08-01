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
            var vRAutomatedReportQueryDefinitionManager = new VRAutomatedReportQueryDefinitionManager();
            return vRAutomatedReportQueryDefinitionManager.DoesUserHaveAccessToAtLeastOneQuery(context.UserId);
        }
    }
}
