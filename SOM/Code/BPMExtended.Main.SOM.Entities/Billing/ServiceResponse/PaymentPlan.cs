using System;

namespace BPMExtended.Main.SOMAPI
{
    public class PaymentPlan
    {
        public string Id { get; set; }
        public string Desc { get; set; }
        public string Status { get; set; }
        public Decimal? Amount { get; set; }
        public Decimal? NextInstallmentAmount { get; set; }
        public Decimal? NextInstallmentInterest { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime NextInstallmentDueDate { get; set; }
    }
}
