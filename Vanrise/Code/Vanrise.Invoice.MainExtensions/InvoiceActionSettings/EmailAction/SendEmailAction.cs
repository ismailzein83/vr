using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class SendEmailAction : InvoiceActionSettings
    {
        public override Guid ConfigId { get { return new Guid("6AB1BA29-F57F-439E-AEA6-7A98AF3FE184"); } }
        public override string ActionTypeName
        {
            get { return "SendEmailAction"; }
        }
        public List<EmailActionAttachment> EmailAttachments { get; set; }
    }
    public class EmailActionAttachment
    {
        public string Title { get; set; }
        public InvoiceFileConverter InvoiceFileConverter { get; set; }
    }
}
