using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class DownloadFileInvoiceAction : InvoiceActionSettings
    {
        public override Guid ConfigId { get { return new Guid("F36A2C18-9033-4F9E-AB7D-0730F630A81A"); } }
        public override string ActionTypeName
        {
            get { return "DownloadFileInvoiceAction"; }
        }
    }
}
