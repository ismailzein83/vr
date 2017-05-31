using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business.CarrierAccounts;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.Invoice.Entities;
using TOne.WhS.BusinessEntity.Business;
namespace TOne.WhS.Invoice.Business
{
    public class InvoiceCarrierAccountStatusChangedHandler : CarrierAccountStatusChangedEventHandler
    {
        public override Guid ConfigId
        {
            get { return new Guid("7B2F94DE-AFF7-47FD-AC27-A9FEDA93886F"); }
        }

        public override void Execute(IVREventHandlerContext context)
        {
            var eventPayload = context.EventPayload as CarrierAccountStatusChangedEventPayload;
            eventPayload.ThrowIfNull("context.EventPayload", eventPayload);
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            var carrierAccount = carrierAccountManager.GetCarrierAccount(eventPayload.CarrierAccountId);
            Vanrise.Invoice.Business.InvoiceAccountManager invoiceAccountManager = new Vanrise.Invoice.Business.InvoiceAccountManager();
            VRAccountStatus vrAccountStatus = VRAccountStatus.Active;
            switch (carrierAccount.CarrierAccountSettings.ActivationStatus)
            {
                case BusinessEntity.Entities.ActivationStatus.Active: vrAccountStatus = VRAccountStatus.Active; break;
                case BusinessEntity.Entities.ActivationStatus.Inactive: vrAccountStatus = VRAccountStatus.InActive; break;
            }
            InvoiceAccountManager carrierInvoiceAccountManager = new Business.InvoiceAccountManager();
            var invoiceAccounts = carrierInvoiceAccountManager.GetInvoiceAccountsByCarrierAccountId(eventPayload.CarrierAccountId);
            if (invoiceAccounts != null)
            {
                foreach (var invoiceAccount in invoiceAccounts)
                {
                    invoiceAccountManager.TryUpdateInvoiceAccount(invoiceAccount.Settings.InvoiceTypeId, invoiceAccount.InvoiceAccountId.ToString(), invoiceAccount.BED, invoiceAccount.EED, vrAccountStatus, false);
                }
            }
        }
    }
}
