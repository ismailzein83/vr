using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Business;
using Vanrise.BusinessProcess;
using Vanrise.Invoice.MainExtensions.InvoiceSettingParts;
namespace Vanrise.Invoice.BP.Activities
{
     public sealed class GeneratePartnersInvoices : BaseCodeActivity
    {
        #region Arguments
        [RequiredArgument]
        public InArgument<Guid> InvoiceTypeId { get; set; }
        [RequiredArgument]
        public InArgument<string> PartnerId { get; set; }
        #endregion
        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            var invoiceTypeId = context.ActivityContext.GetValue(this.InvoiceTypeId);
            var partnerId = context.ActivityContext.GetValue(this.PartnerId);
            InvoiceManager invoiceManager = new InvoiceManager();
            InvoiceSettingManager invoiceSettingManager = new Business.InvoiceSettingManager();
            var issueDate = DateTime.Now.AddYears(1);
            if (partnerId != null)
            {
                PartnerManager partnerManager = new PartnerManager();

                var partnerSetting = partnerManager.GetInvoicePartnerSetting(invoiceTypeId, partnerId);

                AutomaticInvoiceSettingPart automaticInvoiceSettingPart = invoiceSettingManager.GetInvoiceSettingDetailByType<AutomaticInvoiceSettingPart>(partnerSetting.InvoiceSetting.InvoiceSettingId);
                if (automaticInvoiceSettingPart.IsEnabled)
                {
                    var billingPeriod = invoiceManager.GetBillingInterval(invoiceTypeId, partnerId, issueDate);
                    if (billingPeriod != null)
                    {
                        invoiceManager.GenerateInvoice(new Entities.GenerateInvoiceInput
                        {
                            InvoiceTypeId = invoiceTypeId,
                            IssueDate = issueDate,
                            PartnerId = partnerId,
                            FromDate = billingPeriod.FromDate,
                            ToDate = billingPeriod.ToDate
                        });
                    }
                }
            }
        }
    }
}
