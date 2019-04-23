using System;

namespace BPMExtended.Main.SOMAPI
{
    public class Payment
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string GLAccount { get; set; }
        public string PaymentMethodId { get; set; }
        public string Currency { get; set; }
        public DateTime EntryDate { get; set; }
        public decimal CashAmount { get; set; }
        public decimal CurrentAmount { get; set; }
    }
}
