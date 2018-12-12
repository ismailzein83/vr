using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class SendFTPAction : InvoiceActionSettings
    {
        public override Guid ConfigId { get { return new Guid("42550809-9516-4A0E-ACE9-166B1A7CFE71"); } }
        public override string ActionTypeName
        {
            get { return "SendFTPAction"; }
        }

        public override InvoiceActionType Type
        {
            get { return InvoiceActionType.SendFTP; }
        }
    }
}
