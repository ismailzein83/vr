using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Analytics.Data;
using TOne.WhS.Analytics.Entities;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Security.Entities;

namespace TOne.WhS.Analytics.Business
{
    public class VariationReportViewSettings : ViewSettings
    {
        private AnalyticHelper _analyticHelper = new AnalyticHelper();

        public override bool DoesUserHaveAccess(IViewUserAccessContext context)
        {
            int userId = ContextFactory.GetContext().GetLoggedInUserId();
            return _analyticHelper.DoesUserHaveBillingViewAccess(userId) || _analyticHelper.DoesUserHaveTrafficViewAccess(userId);
        }
    }
}
