using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Business;
using Vanrise.BusinessProcess;
using Vanrise.Invoice.Entities;
using Vanrise.Invoice.Business.Context;
using Vanrise.Entities;
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
            InvoiceTypeManager invoiceTypeManager = new Business.InvoiceTypeManager();
           

            var issueDate = DateTime.Now;
            if (partnerId != null)
            {
                var invoiceType = invoiceTypeManager.GetInvoiceType(invoiceTypeId);
                var invoiceTypePartnerManager = invoiceType.Settings.ExtendedSettings.GetPartnerManager();
               

                PartnerManager partnerManager = new PartnerManager();
                var partnerSetting = partnerManager.GetInvoicePartnerSetting(invoiceTypeId, partnerId);

                AutomaticInvoiceSettingPart automaticInvoiceSettingPart = invoiceSettingManager.GetInvoiceSettingDetailByType<AutomaticInvoiceSettingPart>(partnerSetting.InvoiceSetting.InvoiceSettingId);
                if (automaticInvoiceSettingPart.IsEnabled)
                {
                    var billingPeriod = invoiceManager.GetBillingInterval(invoiceTypeId, partnerId, issueDate);
                    if (billingPeriod != null)
                    {
                      var generatedInvoice =  invoiceManager.GenerateInvoice(new Entities.GenerateInvoiceInput
                        {
                            InvoiceTypeId = invoiceTypeId,
                            IssueDate = issueDate,
                            PartnerId = partnerId,
                            FromDate = billingPeriod.FromDate,
                            ToDate = billingPeriod.ToDate
                        });
                      PartnerNameManagerContext PartnerNameManagerContext = new PartnerNameManagerContext
                      {
                          PartnerId = partnerId
                      };
                        var partnerName = invoiceTypePartnerManager.GetPartnerName(PartnerNameManagerContext);
                        if(generatedInvoice.Result == InsertOperationResult.Succeeded)
                        {

                            context.ActivityContext.WriteTrackingMessage(LogEntryType.Information, string.Format("Invoice generated for {0} from period {1} to  period {2}", partnerName, billingPeriod.FromDate.ToShortDateString(), billingPeriod.ToDate.ToShortDateString()));
                        }
                        else
                        {
                            context.ActivityContext.WriteTrackingMessage(LogEntryType.Information, string.Format("Invoice not generated for {0} from period {1} to  period {2} :{3}", partnerName, billingPeriod.FromDate.Date.ToShortDateString(), billingPeriod.ToDate.ToShortDateString(), generatedInvoice.Message));
                        }
                       
                    }
                    
                }
            }
        }
    }
}
