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

        public override InvoiceActionType Type
        {
            get { return InvoiceActionType.SendMail; }
        }
        public Guid InvoiceMailTypeId { get; set; }
        public string InfoType { get; set; }
        public List<Guid> AttachmentsIds { get; set; }
    }
}
