using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Business;
using Vanrise.BusinessProcess;
namespace Vanrise.Invoice.BP.Activities
{
     public sealed class GeneratePartnersInvoices : CodeActivity
    {
        #region Arguments
        [RequiredArgument]
        public InArgument<Guid> InvoiceTypeId { get; set; }
        [RequiredArgument]
        public InArgument<IEnumerable<string>> PartnerIds { get; set; }
        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            var invoiceTypeId = context.GetValue(this.InvoiceTypeId);
            var partnerIds = context.GetValue(this.PartnerIds);
            InvoiceManager invoiceManager = new InvoiceManager();
            invoiceManager.userId = context.GetSharedInstanceData().InstanceInfo.InitiatorUserId;
            var issueDate = DateTime.Now.AddYears(1);
            if(partnerIds != null)
            {
                PartnerManager partnerManager = new PartnerManager();
                foreach(var partnerId in partnerIds)
                {
                    var partnerSetting = partnerManager.GetInvoicePartnerSetting(invoiceTypeId, partnerId);
                    if(partnerSetting.InvoiceSetting.Details.EnableAutomaticInvoice)
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
}
