using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Invoice.Entities
{
    public class OriginalInvoiceDataRuntime
    {
        public Dictionary<int, OriginalDataCurrrency>  OriginalDataCurrency { get; set; }
        public string Reference { get; set; }
        public List<AttachementFileRuntime> AttachementFilesRuntime { get; set; }
    }
    public class OriginalDataCurrrency
    {
        public string CurrencySymbol { get; set; }
        public decimal? OriginalAmount { get; set; }
        public bool IncludeOriginalAmountInSettlement { get; set; }
    }
    public class AttachementFileRuntime
    {
        public long FileId { get; set; }
        public string FileName { get; set; }
    }
}
