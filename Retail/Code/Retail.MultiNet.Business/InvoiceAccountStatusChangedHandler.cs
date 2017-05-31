using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Business.AccountHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
namespace Retail.MultiNet.Business
{
    public class InvoiceAccountStatusChangedHandler : AccountStatusChangedEventHandler
    {
        public override Guid ConfigId
        {
            get { return new Guid("6A39F3AF-DA88-4535-933F-9A34ED81D31A"); }
        }

        public override void Execute(IVREventHandlerContext context)
        {
            var eventPayload = context.EventPayload as AccountStatusChangedEventPayload;
            eventPayload.ThrowIfNull("context.EventPayload", eventPayload);

            AccountBEManager accountBEManager = new AccountBEManager();
            var account = accountBEManager.GetAccount(eventPayload.AccountBEDefinitionId, eventPayload.AccountId);
            Vanrise.Invoice.Business.InvoiceAccountManager invoiceAccountManager = new Vanrise.Invoice.Business.InvoiceAccountManager();
            VRAccountStatus vrAccountStatus = VRAccountStatus.Active;
            FinancialAccountManager financialAccountManager = new FinancialAccountManager();
            FinancialAccountDefinitionManager financialAccountDefinitionManager = new FinancialAccountDefinitionManager();
            var financialAccounts = financialAccountManager.GetFinancialAccounts(eventPayload.AccountBEDefinitionId, eventPayload.AccountId,false);
            if (financialAccounts != null)
            {
                foreach (var financialAccount in financialAccounts)
                {
                    var financialAccountDefinition = financialAccountDefinitionManager.GetFinancialAccountDefinition(financialAccount.FinancialAccount.FinancialAccountDefinitionId);
                    invoiceAccountManager.TryUpdateInvoiceAccount(financialAccountDefinition.Settings.InvoiceTypeId.Value , financialAccount.FinancialAccountId, financialAccount.FinancialAccount.BED, financialAccount.FinancialAccount.EED, vrAccountStatus, false);
                }
            }
        }
    }
}
