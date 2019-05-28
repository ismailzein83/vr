using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Invoice.Entities
{
    public class OriginalInvoiceDataRuntime
    {
        public Dictionary<int, OriginalDataCurrrencyRuntime>  OriginalDataCurrency { get; set; }
        public string Reference { get; set; }
        public List<AttachementFileRuntime> AttachementFilesRuntime { get; set; }
    }
    public class OriginalDataCurrrencyRuntime
    {
        public string CurrencySymbol { get; set; }
        public decimal? OriginalAmount { get; set; }

        decimal? _voiceAmount =null;
        public decimal? TrafficAmount { get { return _voiceAmount; } set { _voiceAmount = value != null ? value : OriginalAmount; } }
        public decimal? SMSAmount { get; set; }
        public decimal? RecurringChargeAmount { get; set; }
        public decimal? DealAmount { get; set; }
        public bool HasTrafficAmount { get; set; }
        public bool HasSMSAmount { get; set; }
        public bool HasRecurringChargeAmount { get; set; }
        public bool HasDealAmount { get; set; }
        public bool IncludeOriginalAmountInSettlement { get; set; }
    }
    public class OriginalDataCurrrency
    {
        public string CurrencySymbol { get; set; }
        public decimal? OriginalAmount { get; set; }
        decimal? _voiceAmount = null;
        public decimal? TrafficAmount { get { return _voiceAmount; } set { _voiceAmount = value != null ? value : OriginalAmount; } }
        public decimal? SMSAmount { get; set; }
        public decimal? RecurringChargeAmount { get; set; }
        public decimal? DealAmount { get; set; }

        public bool IncludeOriginalAmountInSettlement { get; set; }
    }
    public class AttachementFileRuntime
    {
        public long FileId { get; set; }
        public string FileName { get; set; }
    }
}
