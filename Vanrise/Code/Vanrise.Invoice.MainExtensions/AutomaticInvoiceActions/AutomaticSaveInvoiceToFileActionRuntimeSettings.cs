using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Entities;
using Vanrise.Common;
using Vanrise.Invoice.Business.Context;
using Vanrise.Invoice.MainExtensions.AutoGenerateInvoiceActions;
namespace Vanrise.Invoice.MainExtensions.AutomaticInvoiceActions
{
    public class AutomaticSaveInvoiceToFileActionRuntimeSettings : AutomaticInvoiceActionRuntimeSettings
    {
        public List<InvoiceToFileActionSetRuntime> InvoiceToFileActionSets { get; set; }
        public override void Execute(IAutomaticSendEmailActionRuntimeSettingsContext context)
        {
            if (this.InvoiceToFileActionSets != null)
            {
                var invoiceType = new InvoiceTypeManager().GetInvoiceType(context.Invoice.InvoiceTypeId);
                invoiceType.ThrowIfNull("invoiceType", context.Invoice.InvoiceTypeId);
                invoiceType.Settings.ThrowIfNull("invoiceType.Settings");
                invoiceType.Settings.AutomaticInvoiceActions.ThrowIfNull("invoiceType.Settings.AutomaticInvoiceActions");

                var automaticInvoiceAction = invoiceType.Settings.AutomaticInvoiceActions.FindRecord(x => x.AutomaticInvoiceActionId == context.AutomaticInvoiceActionId);
                automaticInvoiceAction.ThrowIfNull("automaticInvoiceAction");
                var automaticSaveInvoiceToFileActionSettings = automaticInvoiceAction.Settings as AutomaticSaveInvoiceToFileAction;
                automaticSaveInvoiceToFileActionSettings.ThrowIfNull("automaticSaveInvoiceToFileActionSettings");
                InvoiceEmailActionManager invoiceEmailActionManager = new InvoiceEmailActionManager();

                foreach (var invoiceToFileActionSet in InvoiceToFileActionSets)
                {
                    if (invoiceToFileActionSet.IsEnabled)
                    {
                        List<VRMailAttachement> vrMailAttachments = new List<VRMailAttachement>();
                        invoiceType.Settings.AutomaticInvoiceActions.ThrowIfNull("invoiceType.Settings.InvoiceAttachments");
                        var emailActionAttachmentSetDefinition = automaticSaveInvoiceToFileActionSettings.InvoiceToFileActionSets.FindRecord(x => x.InvoiceToFileActionSetId == invoiceToFileActionSet.InvoiceToFileActionSetId);
                        var partnerInvoiceFilterConditionContext = new PartnerInvoiceFilterConditionContext
                        {
                            InvoiceType = invoiceType,
                            generateInvoiceInput = new GenerateInvoiceInput
                            {
                                InvoiceTypeId = context.Invoice.InvoiceTypeId,
                                IssueDate = context.Invoice.IssueDate,
                                PartnerId = context.Invoice.PartnerId,
                                FromDate = context.Invoice.FromDate,
                                ToDate = context.Invoice.ToDate,
                            }
                        };
                        if (emailActionAttachmentSetDefinition.FilterCondition == null || emailActionAttachmentSetDefinition.FilterCondition.IsFilterMatch(partnerInvoiceFilterConditionContext))
                        {
                            if (invoiceToFileActionSet.Attachments != null)
                            {
                                invoiceType.Settings.InvoiceAttachments.ThrowIfNull("invoiceType.Settings.InvoiceAttachments");
                                foreach (var attachment in invoiceToFileActionSet.Attachments)
                                {
                                    var invoiceAttachment = invoiceType.Settings.InvoiceAttachments.FindRecord(x => x.InvoiceAttachmentId == attachment.AttachmentId);
                                    invoiceAttachment.ThrowIfNull("invoiceAttachment", attachment.AttachmentId);
                                    invoiceAttachment.InvoiceFileConverter.ThrowIfNull("invoiceAttachment.InvoiceFileConverter");
                                    InvoiceRDLCFileConverterContext invoiceRDLCFileConverterContext = new InvoiceRDLCFileConverterContext
                                    {
                                        InvoiceId = context.Invoice.InvoiceId,
                                    };
                                    var invoicefile = invoiceAttachment.InvoiceFileConverter.ConvertToInvoiceFile(invoiceRDLCFileConverterContext);
                                    if (invoicefile != null)
                                    {
                                        vrMailAttachments.Add(invoicefile.ConvertToAttachment());
                                    }
                                }
                            }
                        }
                    }
           

                }
            }

        }
    }
    public class InvoiceToFileActionSetRuntime
    {
        public Guid InvoiceToFileActionSetId { get; set; }
        public List<InvoiceToFileActionAttachmentRuntime> Attachments { get; set; }
        public bool IsEnabled { get; set; }

    }
    public class InvoiceToFileActionAttachmentRuntime
    {
        public Guid AttachmentId { get; set; }
    }
}
