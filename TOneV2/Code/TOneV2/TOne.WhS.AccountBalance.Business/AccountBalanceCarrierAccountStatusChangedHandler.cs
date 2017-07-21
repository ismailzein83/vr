using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business.CarrierAccounts;
using Vanrise.Entities;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.AccountBalance.Business;
namespace TOne.WhS.AccountBalance.Business
{
    public class AccountBalanceCarrierAccountStatusChangedHandler : CarrierAccountStatusChangedEventHandler
    {
        public override Guid ConfigId
        {
            get { return new Guid("863498D8-FF33-4FC3-8380-A87DEA068AD8"); }
        }

        public override void Execute(IVREventHandlerContext context)
        {
            var eventPayload = context.EventPayload as CarrierAccountStatusChangedEventPayload;
            eventPayload.ThrowIfNull("context.EventPayload", eventPayload);
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            var carrierAccount = carrierAccountManager.GetCarrierAccount(eventPayload.CarrierAccountId);
            LiveBalanceManager liveBalanceManager = new LiveBalanceManager();
            VRAccountStatus vrAccountStatus = VRAccountStatus.Active;
            switch (carrierAccount.CarrierAccountSettings.ActivationStatus)
            {
                case BusinessEntity.Entities.ActivationStatus.Active: vrAccountStatus = VRAccountStatus.Active; break;
                case BusinessEntity.Entities.ActivationStatus.Inactive: vrAccountStatus = VRAccountStatus.InActive; break;
            }
            FinancialAccountManager financialAccountManager = new FinancialAccountManager();
            var financialAccounts = financialAccountManager.GetFinancialAccountsByCarrierAccountId(eventPayload.CarrierAccountId);
            if (financialAccounts != null)
            {
                foreach (var financialAccount in financialAccounts)
                {
                    liveBalanceManager.TryUpdateLiveBalanceStatus(financialAccount.FinancialAccountId.ToString(), financialAccount.Settings.AccountTypeId, vrAccountStatus, false);
                }
            }
        }
    }
}
