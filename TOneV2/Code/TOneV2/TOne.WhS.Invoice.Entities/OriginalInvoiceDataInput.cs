using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Invoice.Entities
{
    public enum InvoiceCarrierType { Customer = 0, Supplier = 1 };
    public class OriginalInvoiceDataInput
    {
        public int InvoiceId { get; set; }
        public string Reference { get; set; }
        public List<AttachementFile> AttachementFiles { get; set; }
        public Dictionary<int, OriginalDataCurrrency> OriginalDataCurrency { get; set; }
        public InvoiceCarrierType invoiceCarrierType { get; set; }

    }
}
