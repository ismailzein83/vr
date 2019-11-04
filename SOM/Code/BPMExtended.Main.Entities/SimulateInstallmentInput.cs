using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class SimulateInstallment
    {
       public SimulateInstallmentInput input { get; set; }
    }

    public class SimulateInstallmentInput
    {
        public string CustomerId { get; set; }
        public string PaymentPlanTemplateId { get; set; }
        public string InvoiceCode { get; set; }
        public string InvoiceAmount { get; set; }
        public string AdditionalFees { get; set; }
        public string LateFee { get; set; }
        public string FirstPayment { get; set; }
        public string ReductionRate { get; set; }
        public string Currency { get; set; }
        public string StartDate { get; set; }
        public string ApprovalId { get; set; }
    }
}
