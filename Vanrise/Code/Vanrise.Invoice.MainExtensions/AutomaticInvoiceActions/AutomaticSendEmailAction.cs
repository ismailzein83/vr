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
        public override void Execute(IAutomaticInvoiceActionSettingsContext contex)
        {
            throw new NotImplementedException();
        }
    }
    public class EmailActionAttachmentSet
    {
        public Guid EmailActionAttachmentSetId { get; set; }
        public string Name { get; set; }
        public List<EmailActionAttachment> EmailAttachments { get; set; }
        public PartnerInvoiceFilterCondition FilterCondition { get; set; }

    }
}
