using Retail.BusinessEntity.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
using Retail.BusinessEntity.Business;

namespace Retail.Invoice.Business
{
    public class InvoiceAccountStatusChangedHandler : AccountStatusChangedEventHandler
    {
        public override Guid ConfigId
        {
            get { return new Guid("B10199C0-6FC7-484B-AA03-C8DE562119D0"); }
        }

        public override void Execute(IVREventHandlerContext context)
        {
            //var eventPayload = context.EventPayload as AccountStatusChangedEventPayload;
            //eventPayload.ThrowIfNull("context.EventPayload", eventPayload);

            //AccountBEManager accountBEManager = new AccountBEManager();
            //var account = accountBEManager.GetAccount(eventPayload.AccountBEDefinitionId, eventPayload.AccountId);
            //Vanrise.Invoice.Business.InvoiceAccountManager invoiceAccountManager = new Vanrise.Invoice.Business.InvoiceAccountManager();
            //VRAccountStatus vrAccountStatus = VRAccountStatus.Active;
            //Guid invoiceTypeId = Guid.NewGuid();
            //invoiceAccountManager.TryUpdateInvoiceAccount(invoiceTypeId, eventPayload.AccountId.ToString(), null, null, vrAccountStatus, false);
        }
    }
}
