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
         [RequiredArgument]
        public InArgument<int> EndDateOffsetFromToday { get; set; }
         [RequiredArgument]
        public InArgument<int> IssueDateOffsetFromToday { get; set; }
        #endregion
        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            var invoiceTypeId = context.ActivityContext.GetValue(this.InvoiceTypeId);
            var partnerId = context.ActivityContext.GetValue(this.PartnerId);
            InvoiceManager invoiceManager = new InvoiceManager();
            InvoiceSettingManager invoiceSettingManager = new Business.InvoiceSettingManager();
            InvoiceTypeManager invoiceTypeManager = new Business.InvoiceTypeManager();
           
            if (partnerId != null)
            {

                var invoiceType = invoiceTypeManager.GetInvoiceType(invoiceTypeId);
                var invoiceTypePartnerManager = invoiceType.Settings.ExtendedSettings.GetPartnerManager();
                
                PartnerManager partnerManager = new PartnerManager();
                var partnerSetting = partnerManager.GetInvoicePartnerSetting(invoiceTypeId, partnerId);
                AutomaticInvoiceSettingPart automaticInvoiceSettingPart = invoiceSettingManager.GetInvoiceSettingDetailByType<AutomaticInvoiceSettingPart>(partnerSetting.InvoiceSetting.InvoiceSettingId);
                if (automaticInvoiceSettingPart.IsEnabled)
                {

                    int endDateOffsetFromToday = this.EndDateOffsetFromToday.Get(context.ActivityContext);
                    int issueDateOffsetFromToday = this.IssueDateOffsetFromToday.Get(context.ActivityContext);
                    var issueDate = DateTime.Today.AddDays(-issueDateOffsetFromToday);

                    var billingPeriod = invoiceManager.GetBillingInterval(invoiceTypeId, partnerId, issueDate);
                    if (billingPeriod != null)
                    {
                        int? timeZoneId = null;
                        if (invoiceType.Settings.UseTimeZone)
                        {
                            timeZoneId = partnerManager.GetPartnerTimeZone(invoiceTypeId, partnerId);
                        }
                        if (CheckIFShouldGenerateInvoice(billingPeriod.ToDate, endDateOffsetFromToday))
                        {
                            var generatedInvoice = invoiceManager.GenerateInvoice(new Entities.GenerateInvoiceInput
                                                  {
                                                      InvoiceTypeId = invoiceTypeId,
                                                      IssueDate = issueDate,
                                                      PartnerId = partnerId,
                                                      FromDate = billingPeriod.FromDate,
                                                      ToDate = new DateTime(billingPeriod.ToDate.Year, billingPeriod.ToDate.Month, billingPeriod.ToDate.Day,23,59,59,998),
                                                      TimeZoneId = timeZoneId
                                                  });
                            PartnerNameManagerContext PartnerNameManagerContext = new PartnerNameManagerContext
                            {
                                PartnerId = partnerId
                            };
                            var partnerName = invoiceTypePartnerManager.GetPartnerName(PartnerNameManagerContext);
                            if (generatedInvoice.Result == InsertOperationResult.Succeeded)
                            {

                                context.ActivityContext.WriteTrackingMessage(LogEntryType.Information, "Invoice generated for {0} from period {1} to  period {2}", partnerName, billingPeriod.FromDate.ToShortDateString(), billingPeriod.ToDate.ToShortDateString());
                            }
                            else
                            {
                                context.ActivityContext.WriteTrackingMessage(LogEntryType.Warning, "Invoice not generated for {0} from period {1} to  period {2} :{3}", partnerName, billingPeriod.FromDate.Date.ToShortDateString(), billingPeriod.ToDate.ToShortDateString(), generatedInvoice.Message);
                            }
                        }
                    }
                }
            }
        }

        private bool CheckIFShouldGenerateInvoice(DateTime toDate, int endDateOffsetFromToday)
        {
            return (toDate <= DateTime.Today.AddDays(-endDateOffsetFromToday));
        }
    }
}
