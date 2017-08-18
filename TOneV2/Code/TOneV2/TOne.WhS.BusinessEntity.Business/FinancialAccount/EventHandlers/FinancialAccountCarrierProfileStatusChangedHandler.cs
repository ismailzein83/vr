using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Business.CarrierProfiles;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Business.FinancialAccount.EventHandlers
{
    public class FinancialAccountCarrierProfileStatusChangedHandler : CarrierProfileStatusChangedEventHandler
    {
        public override Guid ConfigId
        {
            get { return new Guid("402B4AAD-CA07-4EA6-BCBD-6B0DD571B1D7"); }
        }

        static WHSFinancialAccountManager s_financialAccountManager = new WHSFinancialAccountManager();
        static CarrierProfileManager s_carrierProfileManager = new CarrierProfileManager();

        public override void Execute(Vanrise.Entities.IVREventHandlerContext context)
        {
            var eventPayload = context.EventPayload as CarrierProfileStatusChangedEventPayload;
            eventPayload.ThrowIfNull("context.EventPayload", eventPayload);

            VRAccountStatus vrAccountStatus = s_carrierProfileManager.IsCarrierProfileActive(eventPayload.CarrierProfileId) ? VRAccountStatus.Active : VRAccountStatus.InActive;

            var financialAccounts = s_financialAccountManager.GetFinancialAccountsByCarrierProfileId(eventPayload.CarrierProfileId);
            if (financialAccounts != null)
                s_financialAccountManager.ReflectStatusToInvoiceAndBalanceAccounts(vrAccountStatus, financialAccounts);
        }
    }
}
