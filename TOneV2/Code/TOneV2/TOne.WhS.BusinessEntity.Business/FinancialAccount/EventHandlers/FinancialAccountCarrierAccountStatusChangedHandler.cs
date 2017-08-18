using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Business.CarrierAccounts;
using Vanrise.AccountBalance.Business;
using Vanrise.Entities;
using Vanrise.Invoice.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business.FinancialAccount.EventHandlers
{
    public class FinancialAccountCarrierAccountStatusChangedHandler : CarrierAccountStatusChangedEventHandler
    {
        public override Guid ConfigId
        {
            get { return new Guid("3B2F76E5-C3EA-4D57-AB8E-FF261BCEC100"); }
        }

        static WHSFinancialAccountManager s_financialAccountManager = new WHSFinancialAccountManager();
        static CarrierAccountManager s_carrierAccountManager = new CarrierAccountManager();
        public override void Execute(Vanrise.Entities.IVREventHandlerContext context)
        {
            var eventPayload = context.EventPayload as CarrierAccountStatusChangedEventPayload;
            eventPayload.ThrowIfNull("context.EventPayload", eventPayload);
           
            VRAccountStatus vrAccountStatus = s_carrierAccountManager.IsCarrierAccountActive(eventPayload.CarrierAccountId) ? VRAccountStatus.Active : VRAccountStatus.InActive;
                        
            var financialAccounts = s_financialAccountManager.GetFinancialAccountsByCarrierAccountId(eventPayload.CarrierAccountId);
            if (financialAccounts != null)
                s_financialAccountManager.ReflectStatusToInvoiceAndBalanceAccounts(vrAccountStatus, financialAccounts);
        }

        
    }
}
