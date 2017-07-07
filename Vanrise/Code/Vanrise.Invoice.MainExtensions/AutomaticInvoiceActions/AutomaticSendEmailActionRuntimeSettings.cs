using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions.AutomaticInvoiceActions
{
    public class AutomaticSendEmailActionRuntimeSettings : AutomaticInvoiceActionRuntimeSettings
    {
        public List<EmailActionAttachmentSetRuntime> EmailActionAttachmentSets { get; set; }
        public Guid? MailMessageTemplateId { get; set; }
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
