using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Business.CarrierProfiles;
using Vanrise.AccountBalance.Business;
using Vanrise.Common;
using Vanrise.Entities;
namespace TOne.WhS.AccountBalance.Business
{
    public class AccountBalanceCarrierProfileStatusChangedHandler : CarrierProfileStatusChangedEventHandler
    {
        public override Guid ConfigId
        {
            get { return new Guid("B9F12E8F-0057-47DF-886D-2BA71EBE9D95"); }
        }

        public override void Execute(Vanrise.Entities.IVREventHandlerContext context)
        {
            var eventPayload = context.EventPayload as CarrierProfileStatusChangedEventPayload;
            eventPayload.ThrowIfNull("context.EventPayload", eventPayload);
            CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
            var carrierProfile = carrierProfileManager.GetCarrierProfile(eventPayload.CarrierProfileId);
            carrierProfile.ThrowIfNull("carrierProfile", eventPayload.CarrierProfileId);
            carrierProfile.Settings.ThrowIfNull("carrierProfile.Settings");

            LiveBalanceManager liveBalanceManager = new LiveBalanceManager();
            VRAccountStatus vrAccountStatus = VRAccountStatus.Active;
            switch (carrierProfile.Settings.ActivationStatus)
            {
                case BusinessEntity.Entities.CarrierProfileActivationStatus.Active: vrAccountStatus = VRAccountStatus.Active; break;
                case BusinessEntity.Entities.CarrierProfileActivationStatus.InActive: vrAccountStatus = VRAccountStatus.InActive; break;
            }
            FinancialAccountManager financialAccountManager = new FinancialAccountManager();
            var financialAccounts = financialAccountManager.GetCarrierProfileFinancialAccounts(eventPayload.CarrierProfileId);
            if (financialAccounts != null)
            {
                foreach (var financialAccount in financialAccounts)
                {
                    if (vrAccountStatus == VRAccountStatus.InActive && (!financialAccount.EED.HasValue || financialAccount.EED.Value > DateTime.Now))
                    {
                        var lastTransactionDate = new BillingTransactionManager().GetLastTransactionDate(financialAccount.Settings.AccountTypeId, financialAccount.FinancialAccountId.ToString());
                        if (lastTransactionDate.HasValue)
                        {
                            financialAccount.EED = lastTransactionDate.Value;
                        }
                        if (!lastTransactionDate.HasValue || financialAccount.EED < financialAccount.BED)
                        {
                            financialAccount.EED = financialAccount.BED;
                        }
                        financialAccountManager.UpdateFinancialAccount(financialAccount);
                    }
                    liveBalanceManager.TryUpdateLiveBalanceStatus(financialAccount.FinancialAccountId.ToString(), financialAccount.Settings.AccountTypeId, vrAccountStatus, false);
                }
            }
        }
    }
}
