using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions.AutoGenerateInvoiceActions
{
    public class AutomaticSendEmailAction : AutoGenerateInvoiceActionSettings
    {
        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }
        public List<EmailActionAttachment> EmailAttachments { get; set; }
        public override void Execute(IAutoGenerateInvoiceActionSettingsContext contex)
        {
            throw new NotImplementedException();
        }
    }
}
