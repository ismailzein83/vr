using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions.InvoiceSettingParts
{
    public class DuePeriodInvoiceSettingPart : InvoiceSettingPart
    {
        public int DuePeriod { get; set; }
        public override Guid ConfigId
        {
            get { return new Guid("49ec6bb0-0c46-4610-93d0-4ed0be537265"); }
        }
    }
}
