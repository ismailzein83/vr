using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Invoice.Entities
{
    public class OriginalInvoiceDataRuntime
    {
        public string Reference { get; set; }
        public decimal? OriginalAmount { get; set; }
        public List<AttachementFileRuntime> AttachementFilesRuntime { get; set; }
        public bool IncludeOriginalAmountInSettlement { get; set; }
    }
    public class AttachementFileRuntime
    {
        public long FileId { get; set; }
        public string FileName { get; set; }
    }
}
