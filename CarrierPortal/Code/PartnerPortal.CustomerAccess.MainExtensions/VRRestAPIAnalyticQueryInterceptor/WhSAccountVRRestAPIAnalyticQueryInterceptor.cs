using CP.WhS.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.APIEntities;
using Vanrise.Analytic.Entities;
using Vanrise.Common.Business;
using Vanrise.Security.Business;

namespace PartnerPortal.CustomerAccess.MainExtensions.VRRestAPIAnalyticQueryInterceptor
{
    public enum CarrierAccountType { Exchange = 1, Supplier = 2, Customer = 3 };
    public class WhSAccountVRRestAPIAnalyticQueryInterceptor : Vanrise.Analytic.Entities.VRRestAPIAnalyticQueryInterceptor
    {
        public override Guid ConfigId { get { return new Guid("2cf9eade-97ab-467f-b24b-35345324930a"); } }
        public CarrierAccountType AccountType { get; set; }
        public string AccountDimensionName { get; set; }
        public override void PrepareQuery(IVRRestAPIAnalyticQueryInterceptorContext context)
        {
            var userId = SecurityContext.Current.GetLoggedInUserId();
            PortalConnectionManager portalConnectionManager = new PortalConnectionManager();
            var vrConnectionSettings = portalConnectionManager.GetConnectionSettings(context.VRConnectionId);
            var carrierAccounts = vrConnectionSettings.Get<CarrierProfileCarrierAccounts>(string.Format("/api/WhS_BE/CarrierProfile/GetCarrierProfileCarrierAccountsByUserId?userId={0}", userId));
            if(carrierAccounts!=null && carrierAccounts.CarrierAccountIds!=null && carrierAccounts.CarrierAccountIds.Count > 0)
            {
                if (context.Query.Filters == null)
                    context.Query.Filters = new List<DimensionFilter>();

                context.Query.Filters.Add(new DimensionFilter()
                {
                    Dimension = AccountDimensionName,
                    FilterValues = carrierAccounts.CarrierAccountIds.Select<int, object>(x => x).ToList()
                });
            }
        }
    }
}
