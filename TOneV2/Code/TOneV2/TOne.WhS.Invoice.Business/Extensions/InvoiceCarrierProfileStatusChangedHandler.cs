using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Business.CarrierProfiles;
using Vanrise.Invoice.Entities;
using Vanrise.Common;
using Vanrise.Entities;
namespace TOne.WhS.Invoice.Business
{
    public class InvoiceCarrierProfileStatusChangedHandler : CarrierProfileStatusChangedEventHandler
    {
        public override Guid ConfigId
        {
            get { return new Guid("3B0AB1F5-3EB4-411C-8197-41646D734D00"); }
        }

        public override void Execute(Vanrise.Entities.IVREventHandlerContext context)
        {
            var eventPayload = context.EventPayload as CarrierProfileStatusChangedEventPayload;
            eventPayload.ThrowIfNull("context.EventPayload", eventPayload);
            CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
            var carrierProfile = carrierProfileManager.GetCarrierProfile(eventPayload.CarrierProfileId);
            Vanrise.Invoice.Business.InvoiceAccountManager invoiceAccountManager = new Vanrise.Invoice.Business.InvoiceAccountManager();
            VRAccountStatus vrAccountStatus = VRAccountStatus.Active;
            switch (carrierProfile.Settings.ActivationStatus)
            {
                case BusinessEntity.Entities.CarrierProfileActivationStatus.Active: vrAccountStatus = VRAccountStatus.Active; break;
                case BusinessEntity.Entities.CarrierProfileActivationStatus.InActive: vrAccountStatus = VRAccountStatus.InActive; break;
            }
            InvoiceAccountManager carrierInvoiceAccountManager = new Business.InvoiceAccountManager();
            var invoiceAccounts = carrierInvoiceAccountManager.GetCarrierProfileInvoiceAccounts(eventPayload.CarrierProfileId);
            if (invoiceAccounts != null)
            {
                foreach (var invoiceAccount in invoiceAccounts)
                {
                    if (vrAccountStatus == VRAccountStatus.InActive && (!invoiceAccount.EED.HasValue || invoiceAccount.EED.Value > DateTime.Now))
                    {
                        var lastInvoiceToDate =new Vanrise.Invoice.Business.InvoiceManager().GetLastInvoiceToDate(invoiceAccount.Settings.InvoiceTypeId, invoiceAccount.InvoiceAccountId.ToString());
                        if(lastInvoiceToDate.HasValue)
                        {
                            invoiceAccount.EED = lastInvoiceToDate.Value; 
                        }else
                        {
                            invoiceAccount.EED = invoiceAccount.BED;
                        }
                        carrierInvoiceAccountManager.UpdateInvoiceAccount(invoiceAccount);
                    }
                    invoiceAccountManager.TryUpdateInvoiceAccountStatus(invoiceAccount.Settings.InvoiceTypeId, invoiceAccount.InvoiceAccountId.ToString(),vrAccountStatus, false);
                }
            }
        }
    }
}
