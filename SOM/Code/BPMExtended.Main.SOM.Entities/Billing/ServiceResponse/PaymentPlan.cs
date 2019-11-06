using System;

namespace BPMExtended.Main.SOMAPI
{
    public class PaymentPlan
    {
        public string Id { get; set; }
        public string Desc { get; set; }
        public string Status { get; set; }
        public string Amount { get; set; }
        public string NextInstallmentAmount { get; set; }
        public string NextInstallmentInterest { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string NextInstallmentDueDate { get; set; }
    }
}
