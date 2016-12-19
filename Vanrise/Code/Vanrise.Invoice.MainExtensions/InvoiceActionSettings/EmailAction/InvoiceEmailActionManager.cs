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
            if (sendEmailAction != null && sendEmailAction.EmailAttachments != null)
            {

                foreach (var attachement in sendEmailAction.EmailAttachments)
                {
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
            vrMailManager.SendMail(input.EmailTemplate.To, input.EmailTemplate.CC, input.EmailTemplate.Subject, input.EmailTemplate.Body, vrMailAttachments);
        }
         public VRMailEvaluatedTemplate GetEmailTemplate(long invoiceId)
         {
             InvoiceManager invoiceManager = new InvoiceManager();

             var invoice = invoiceManager.GetInvoice(invoiceId);
             InvoiceTypeManager manager = new InvoiceTypeManager();
             var invoiceType = manager.GetInvoiceType(invoice.InvoiceTypeId);
             InvoiceTypeExtendedSettingsInfoContext context = new InvoiceTypeExtendedSettingsInfoContext
             {
                 InfoType = "MailTemplate",
                 Invoice = invoice

             };
             return invoiceType.Settings.ExtendedSettings.GetInfo(context);
         }

         public IEnumerable<SendEmailAttachmentTypeConfig> GetSendEmailAttachmentTypeConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<SendEmailAttachmentTypeConfig>(SendEmailAttachmentTypeConfig.EXTENSION_TYPE);
        }
    }
}
