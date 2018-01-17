using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class MinAmountInvoiceSettingPart : InvoiceSettingPart
    {
        public decimal? MinAmount { get; set; }
        public override Guid ConfigId
        {
            get { return new Guid("FCFB637A-A6A4-4B2F-A817-9FF9C3AF3BAB"); }
        }
    }
}
