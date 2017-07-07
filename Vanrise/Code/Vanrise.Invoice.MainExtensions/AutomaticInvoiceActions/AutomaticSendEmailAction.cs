using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions.AutoGenerateInvoiceActions
{
    public class AutomaticSendEmailAction : AutomaticInvoiceActionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("5BAC5D43-DA8F-4996-9730-72132FD83ECB"); }
        }
        public List<EmailActionAttachmentSet> EmailActionAttachmentSets { get; set; }
        public Guid? MailMessageTypeId { get; set; }
        public override void Execute(IAutomaticInvoiceActionSettingsContext contex)
        {
            throw new NotImplementedException();
        }
        public override string RuntimeEditor
        {
            get { return "vr-invoicetype-automaticinvoiceaction-sendemail-runtime"; }
        }
    }
    public class EmailActionAttachmentSet
    {
        public Guid EmailActionAttachmentSetId { get; set; }
        public string Name { get; set; }
        public List<Guid> AttachmentsIds { get; set; }
        public Guid? MailMessageTypeId { get; set; }
        public PartnerInvoiceFilterCondition FilterCondition { get; set; }

    }
}
