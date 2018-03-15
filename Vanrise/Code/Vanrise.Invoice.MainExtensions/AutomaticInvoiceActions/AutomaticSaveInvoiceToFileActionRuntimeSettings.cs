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
using System.IO;
namespace Vanrise.Invoice.MainExtensions.AutomaticInvoiceActions
{
    public class AutomaticSaveInvoiceToFileActionRuntimeSettings : AutomaticInvoiceActionRuntimeSettings
    {
        public List<InvoiceToFileActionSetRuntime> InvoiceToFileActionSets { get; set; }
        public string LocationPath { get; set; }
        public override void Execute(IAutomaticActionRuntimeSettingsContext context)
        {
            if (this.InvoiceToFileActionSets != null)
            {
                var invoiceType = new InvoiceTypeManager().GetInvoiceType(context.Invoice.InvoiceTypeId);
                invoiceType.ThrowIfNull("invoiceType", context.Invoice.InvoiceTypeId);
                invoiceType.Settings.ThrowIfNull("invoiceType.Settings");
              //  invoiceType.Settings.AutomaticInvoiceActions.ThrowIfNull("invoiceType.Settings.AutomaticInvoiceActions");

               // var automaticInvoiceAction = invoiceType.Settings.AutomaticInvoiceActions.FindRecord(x => x.AutomaticInvoiceActionId == context.AutomaticInvoiceActionId);
              //  automaticInvoiceAction.ThrowIfNull("automaticInvoiceAction");
                var automaticSaveInvoiceToFileActionSettings = context.DefinitionSettings as AutomaticSaveInvoiceToFileAction;
                automaticSaveInvoiceToFileActionSettings.ThrowIfNull("automaticSaveInvoiceToFileActionSettings");
                InvoiceEmailActionManager invoiceEmailActionManager = new InvoiceEmailActionManager();
                if (InvoiceToFileActionSets == null || InvoiceToFileActionSets.Count == 0 || !InvoiceToFileActionSets.Any(x => x.IsEnabled))
                {
                    context.ErrorMessage = "Cannot save invoice to file. Reason: no file options specified.";
                    context.IsErrorOccured = false;
                }
                bool actionExecuted = false;
                foreach (var invoiceToFileActionSet in InvoiceToFileActionSets)
                {
                    if (invoiceToFileActionSet.IsEnabled)
                    {
                       // invoiceType.Settings.AutomaticInvoiceActions.ThrowIfNull("invoiceType.Settings.InvoiceAttachments");
                        var emailActionAttachmentSetDefinition = automaticSaveInvoiceToFileActionSettings.InvoiceToFileActionSets.FindRecord(x => x.InvoiceToFileActionSetId == invoiceToFileActionSet.InvoiceToFileActionSetId);
                        var partnerInvoiceFilterConditionContext = new PartnerInvoiceFilterConditionContext
                        {
                            InvoiceType = invoiceType,
                            generateInvoiceInput = new GenerateInvoiceInput
                            {
                                InvoiceTypeId = context.Invoice.InvoiceTypeId,
                                IssueDate = context.Invoice.IssueDate,
                                PartnerId = context.Invoice.PartnerId,
                                Items = new List<GenerateInvoiceInputItem>
                                {
                                    new GenerateInvoiceInputItem
                                    {
                                        FromDate = context.Invoice.FromDate,
                                        ToDate = context.Invoice.ToDate,
                                    }
                                },
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
                                        if (!string.IsNullOrEmpty(this.LocationPath))
                                        {
                                            string fileName = string.Format("{0}.{1}", invoicefile.Name, invoicefile.ExtensionType);
                                            string fullpath = Path.Combine(this.LocationPath, fileName);
                                            File.WriteAllBytes(fullpath, invoicefile.Content);
                                           
                                        }
                                        else
                                        {
                                            context.ErrorMessage = "Cannot save invoice to file. Reason: 'Location Path' is empty.";
                                            context.IsErrorOccured = false;
                                        }
                                    }
                                }
                                actionExecuted = true;
                            }
                        }
                    }
                }
                if (!actionExecuted)
                {
                    context.ErrorMessage = "Cannot save invoice to file. Reason: no file option specified.";
                    context.IsErrorOccured = false;
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
