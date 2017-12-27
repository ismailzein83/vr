using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Analytics.Entities;
using Vanrise.Security.Entities;

namespace TOne.WhS.Analytics.Business
{
    public class AnalyticHelper
    {
        private const string PERMISSION_BILLING_VIEW = "BillingData: View";
        private const string PERMISSION_TRAFFIC_VIEW = "TrafficData: View";

        #region Public Methods
       
        public bool DoesUserHaveBillingViewAccess(int userId)
        {            
            return ContextFactory.GetContext().IsAllowed(PERMISSION_BILLING_VIEW, userId);
        }
        public bool DoesUserHaveTrafficViewAccess(int userId)
        {
            return ContextFactory.GetContext().IsAllowed(PERMISSION_TRAFFIC_VIEW, userId);
        }

        #endregion
    }
}
