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
    public class AutomaticSendEmailActionRuntimeSettings : AutomaticInvoiceActionRuntimeSettings
    {
        public List<EmailActionAttachmentSetRuntime> EmailActionAttachmentSets { get; set; }
        public Guid? MailMessageTemplateId { get; set; }
        public override void Execute(IAutomaticSendEmailActionRuntimeSettingsContext context)
        {
            if (this.EmailActionAttachmentSets != null)
            {
                var invoiceType = new InvoiceTypeManager().GetInvoiceType(context.Invoice.InvoiceTypeId);
                invoiceType.ThrowIfNull("invoiceType", context.Invoice.InvoiceTypeId);
                invoiceType.Settings.ThrowIfNull("invoiceType.Settings");
                invoiceType.Settings.AutomaticInvoiceActions.ThrowIfNull("invoiceType.Settings.AutomaticInvoiceActions");

                var automaticInvoiceAction = invoiceType.Settings.AutomaticInvoiceActions.FindRecord(x => x.AutomaticInvoiceActionId == context.AutomaticInvoiceActionId);
                automaticInvoiceAction.ThrowIfNull("automaticInvoiceAction");
                var automaticSendEmailActionSettings = automaticInvoiceAction.Settings as AutomaticSendEmailAction;
                automaticSendEmailActionSettings.ThrowIfNull("automaticSendEmailActionSettings");
                VRMailManager vrMailManager = new VRMailManager();
                InvoiceEmailActionManager invoiceEmailActionManager = new InvoiceEmailActionManager();


                foreach (var emailActionAttachmentSet in EmailActionAttachmentSets)
                {
                    List<VRMailAttachement> vrMailAttachments = new List<VRMailAttachement>();
                    invoiceType.Settings.AutomaticInvoiceActions.ThrowIfNull("invoiceType.Settings.InvoiceAttachments");
                    var emailActionAttachmentSetDefinition = automaticSendEmailActionSettings.EmailActionAttachmentSets.FindRecord(x => x.EmailActionAttachmentSetId == emailActionAttachmentSet.EmailActionAttachmentSetId);
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
                            TimeZoneId = context.Invoice.TimeZoneId
                        }
                    };
                    if (emailActionAttachmentSetDefinition.FilterCondition == null || emailActionAttachmentSetDefinition.FilterCondition.IsFilterMatch(partnerInvoiceFilterConditionContext))
                    {
                        if (emailActionAttachmentSet.Attachments != null)
                        {
                            invoiceType.Settings.InvoiceAttachments.ThrowIfNull("invoiceType.Settings.InvoiceAttachments");
                            foreach (var attachment in emailActionAttachmentSet.Attachments)
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
                        Guid mailMessageTemplateId;
                        if (emailActionAttachmentSet.MailMessageTemplateId.HasValue)
                        {
                            mailMessageTemplateId = emailActionAttachmentSet.MailMessageTemplateId.Value;
                        }
                        else if (this.MailMessageTemplateId.HasValue)
                        {
                            mailMessageTemplateId = this.MailMessageTemplateId.Value;
                        }
                        else
                        {
                            continue;
                        }
                        var emailTemplateEvaluator = invoiceEmailActionManager.GetEmailTemplate(context.Invoice, mailMessageTemplateId);
                        vrMailManager.SendMail(emailTemplateEvaluator.To, emailTemplateEvaluator.CC, emailTemplateEvaluator.BCC, emailTemplateEvaluator.Subject, emailTemplateEvaluator.Body, vrMailAttachments);
                    }

                }
            }

        }
    }
    public class EmailActionAttachmentSetRuntime
    {
        public Guid EmailActionAttachmentSetId { get; set; }
        public List<EmailActionAttachmentRuntime> Attachments { get; set; }
        public Guid? MailMessageTemplateId { get; set; }

    }
    public class EmailActionAttachmentRuntime
    {
        public Guid AttachmentId { get; set; }
    }
}
