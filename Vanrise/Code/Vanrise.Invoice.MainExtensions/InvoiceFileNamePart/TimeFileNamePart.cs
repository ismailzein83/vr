using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions.InvoiceFileNamePart
{
    public class TimeFileNamePart : VRConcatenatedPartSettings<IInvoiceFileNamePartContext>
    {
        public override Guid ConfigId { get { return new Guid("B63AD817-6776-4883-A522-4F6D045DC67C"); } }
        public string DateTimeFormat { get; set; }
        public override string GetPartText(IInvoiceFileNamePartContext context)
        {
            return DateTime.Now.ToString(this.DateTimeFormat);
        }
    }
}
