using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Business.Context;
using Vanrise.Invoice.Business.Extensions;
using Vanrise.Invoice.Entities;
using Vanrise.Common;
namespace Vanrise.Invoice.MainExtensions
{
    public class InvoiceEmailActionManager
    {
         public void SendEmail(SendEmailActionInput input)
        {
            InvoiceManager invoiceManager = new InvoiceManager();
            var invoice = invoiceManager.GetInvoice(input.InvoiceId);
            InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
            var invoiceType = invoiceTypeManager.GetInvoiceType(invoice.InvoiceTypeId);
            var action = invoiceType.Settings.InvoiceActions.FirstOrDefault(x => x.InvoiceActionId == input.InvoiceActionId);
            SendEmailAction sendEmailAction =null;
            if(action !=  null)
            {
                sendEmailAction = action.Settings as SendEmailAction;
            }
            List<VRMailAttachement> vrMailAttachments = new List<Vanrise.Entities.VRMailAttachement>();
            if (sendEmailAction != null && sendEmailAction.AttachmentsIds != null && sendEmailAction.AttachmentsIds.Count > 0)
            {
                invoiceType.Settings.InvoiceAttachments.ThrowIfNull("invoiceType.Settings.InvoiceAttachments");
                foreach (var attachementId in sendEmailAction.AttachmentsIds)
                {
                    var attachement = invoiceType.Settings.InvoiceAttachments.FindRecord(x => x.InvoiceAttachmentId == attachementId);
                    attachement.ThrowIfNull("attachment");
                    InvoiceRDLCFileConverterContext context = new InvoiceRDLCFileConverterContext
                    {
                        InvoiceId = input.InvoiceId
                    };
                    var invoicefile = attachement.InvoiceFileConverter.ConvertToInvoiceFile(context);
                    if (invoicefile != null)
                    {
                        vrMailAttachments.Add(invoicefile.ConvertToAttachment());
                    }
                }
            }
            VRMailManager vrMailManager = new VRMailManager();
            if (input.AttachementFileIds != null && input.AttachementFileIds.Count > 0)
            {
                foreach(var attachementFileId in input.AttachementFileIds)
                {
                    vrMailAttachments.Add(vrMailManager.ConvertToGeneralAttachement(attachementFileId));
                }
            }
            vrMailManager.SendMail(input.EmailTemplate.From, input.EmailTemplate.To, input.EmailTemplate.CC, input.EmailTemplate.BCC, input.EmailTemplate.Subject, input.EmailTemplate.Body, vrMailAttachments);
            new InvoiceManager().SetInvoiceSentDate(input.InvoiceId, true);
        }
         public EmailTemplateRuntimeEditor GetEmailTemplate(long invoiceId, Guid invoiceMailTemplateId, Guid invoiceActionId)
         {
             EmailTemplateRuntimeEditor emailTemplateRuntimeEditor = new MainExtensions.EmailTemplateRuntimeEditor();

             InvoiceManager invoiceManager = new InvoiceManager();
             var invoice = invoiceManager.GetInvoice(invoiceId);
             invoice.ThrowIfNull("invoice", invoiceId);
             InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
             var invoiceAction = invoiceTypeManager.GetInvoiceAction(invoice.InvoiceTypeId, invoiceActionId);
             invoiceAction.ThrowIfNull("invoiceAction", invoiceActionId);
             var emailInvoiceAction = invoiceAction.Settings as SendEmailAction;
             emailInvoiceAction.ThrowIfNull("emailInvoiceAction");

             PartnerManager partnerManager = new PartnerManager();
             var invoiceSetting = partnerManager.GetInvoicePartnerSetting(invoice.InvoiceTypeId, invoice.PartnerId);

             InvoiceSettingManager invoiceSettingManager = new Business.InvoiceSettingManager();

             var attachmentName = new PartnerManager().EvaluateInvoiceFileNamePattern(invoice.InvoiceTypeId, invoice.PartnerId, invoice);
             if (emailInvoiceAction.AttachmentsIds != null)
             {
                 emailTemplateRuntimeEditor.EmailAttachments = new List<EmailAttachment>();
                 foreach(var emailattachment in emailInvoiceAction.AttachmentsIds)
                 {
                     emailTemplateRuntimeEditor.EmailAttachments.Add(new EmailAttachment
                     {
                         AttachmentId = emailattachment,
                         AttachmentName = attachmentName
                     });
                 }
             }
             emailTemplateRuntimeEditor.VRMailEvaluatedTemplate = GetEmailTemplate( invoice,  invoiceMailTemplateId);
             return emailTemplateRuntimeEditor;
         }
         public VRMailEvaluatedTemplate GetEmailTemplate(Entities.Invoice invoice, Guid invoiceMailTemplateId)
         {
             InvoiceTypeManager manager = new InvoiceTypeManager();
             var invoiceType = manager.GetInvoiceType(invoice.InvoiceTypeId);
             InvoiceTypeExtendedSettingsInfoContext context = new InvoiceTypeExtendedSettingsInfoContext
             {
                 InfoType = "MailTemplate",
                 Invoice = invoice
             };
             VRMailManager vrMailManager = new VRMailManager();
             var objects = invoiceType.Settings.ExtendedSettings.GetInfo(context);
             return vrMailManager.EvaluateMailTemplate(invoiceMailTemplateId, objects);
         }
         public VRMailAttachement DownloadAttachment(long invoiceId, Guid attachmentId)
         {
             InvoiceManager invoiceManager = new InvoiceManager();
             var invoice = invoiceManager.GetInvoice(invoiceId);
             invoice.ThrowIfNull("invoice", invoiceId);
             InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
             var invoiceAttachment = invoiceTypeManager.GeInvoiceTypeAttachment(invoice.InvoiceTypeId, attachmentId);
             invoiceAttachment.ThrowIfNull("invoiceAttachment", attachmentId);
             InvoiceRDLCFileConverterContext context = new InvoiceRDLCFileConverterContext{
                 InvoiceId = invoiceId
             };
             var invoiceFile = invoiceAttachment.InvoiceFileConverter.ConvertToInvoiceFile(context);
             invoiceFile.ThrowIfNull("invoiceFile");
             return invoiceFile.ConvertToAttachment();
         }
         public IEnumerable<SendEmailAttachmentTypeConfig> GetSendEmailAttachmentTypeConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<SendEmailAttachmentTypeConfig>(SendEmailAttachmentTypeConfig.EXTENSION_TYPE);
        }
    }
}
