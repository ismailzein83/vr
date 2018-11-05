using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.APIEntities;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Security.Business;

namespace CP.WhS.Business
{
    public class WhSAccountVRRestAPIRecordQueryInterceptor : VRRestAPIRecordQueryInterceptor
    {
        public override Guid ConfigId { get { return new Guid("0bf49c64-f195-4b0f-9c48-68bec9d41b9d"); }}
        public CarrierAccountType AccountType { get; set; }
        public string FieldName { get; set; }
        public override void PrepareQuery(IVRRestAPIRecordQueryInterceptorContext context)
        {
            var userId = SecurityContext.Current.GetLoggedInUserId();
            PortalConnectionManager portalConnectionManager = new PortalConnectionManager();
            var vrConnectionSettings = portalConnectionManager.GetConnectionSettings(context.VRConnectionId);
            var carrierAccounts = vrConnectionSettings.Get<CarrierProfileCarrierAccounts>(string.Format("/api/WhS_BE/CarrierProfile/GetCarrierProfileCarrierAccountsByUserId?userId={0}", userId));
            if (carrierAccounts != null && carrierAccounts.CarrierAccountIds != null && carrierAccounts.CarrierAccountIds.Count > 0)
            {
                if (context.Query.Filters == null)
                    context.Query.Filters = new List<DataRecordFilter>();
                context.Query.Filters.Add(new DataRecordFilter()
                {
                    FieldName = FieldName,
                    FilterValues = carrierAccounts.CarrierAccountIds.Select<int, object>(x => x).ToList()
                });
            }
        }
    }
}
