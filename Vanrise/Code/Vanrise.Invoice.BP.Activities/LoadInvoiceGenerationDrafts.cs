using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.Invoice.Business;
using Vanrise.BusinessProcess;
using Vanrise.Invoice.Entities;
using Vanrise.Common;
using System.Linq;
namespace Vanrise.Invoice.BP.Activities
{
    public sealed class LoadInvoiceGenerationDrafts : BaseCodeActivity
    {
        [RequiredArgument]
        public InArgument<Guid> InvoiceGenerationIdentifier { get; set; }

        [RequiredArgument]
        public OutArgument<List<PartnerInvoiceGenerationDraft>> PartnerInvoiceGenerationDraftList { get; set; }

       [RequiredArgument]
        public OutArgument<int> TotalPartnerInvoiceGenerationDrafts { get; set; }

        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            Guid invoiceGenerationIdentifier = context.ActivityContext.GetValue(this.InvoiceGenerationIdentifier);
            InvoiceGenerationDraftManager invoiceGenerationDraftManager = new InvoiceGenerationDraftManager();
            List<InvoiceGenerationDraft> invoiceGenerationDraftList = invoiceGenerationDraftManager.GetInvoiceGenerationDrafts(invoiceGenerationIdentifier);

            int totalPartnerInvoiceGenerationDrafts = 0;

            Dictionary<string, PartnerInvoiceGenerationDraft> partnerInvoiceGenerationDraftDic = new Dictionary<string, PartnerInvoiceGenerationDraft>();

            if (invoiceGenerationDraftList != null)
            {
                foreach (var invoiceGenerationDraft in invoiceGenerationDraftList)
                {
                    totalPartnerInvoiceGenerationDrafts++;

                    var partnerInvoiceGenerationDraft = partnerInvoiceGenerationDraftDic.GetOrCreateItem(invoiceGenerationDraft.PartnerId,()=> {
                        return new PartnerInvoiceGenerationDraft
                        {
                            Items = new List<PartnerInvoiceGenerationDraftItem>()
                        };
                    });

                    partnerInvoiceGenerationDraft.InvoiceGenerationIdentifier = invoiceGenerationIdentifier;
                    partnerInvoiceGenerationDraft.InvoiceTypeId = invoiceGenerationDraft.InvoiceTypeId;
                    partnerInvoiceGenerationDraft.PartnerId = invoiceGenerationDraft.PartnerId;
                    partnerInvoiceGenerationDraft.PartnerName = invoiceGenerationDraft.PartnerName;
                    partnerInvoiceGenerationDraft.Items.Add(new PartnerInvoiceGenerationDraftItem
                    {
                        CustomPayload = invoiceGenerationDraft.CustomPayload,
                        From = invoiceGenerationDraft.From,
                        InvoiceGenerationDraftId = invoiceGenerationDraft.InvoiceGenerationDraftId,
                        To= invoiceGenerationDraft.To
                    });

                }
            }

            this.PartnerInvoiceGenerationDraftList.Set(context.ActivityContext, partnerInvoiceGenerationDraftDic.Values.ToList());
            this.TotalPartnerInvoiceGenerationDrafts.Set(context.ActivityContext, totalPartnerInvoiceGenerationDrafts);

        }
    }
}