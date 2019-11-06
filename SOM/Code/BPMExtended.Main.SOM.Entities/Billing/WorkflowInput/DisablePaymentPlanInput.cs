using BPMExtended.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.SOMAPI
{
    public class DisablePaymentPlanInput
    {
        public CommonInputArgument CommonInputArgument { get; set; }
        public PaymentData PaymentData { get; set; }
        public string Note { get; set; }
        public DisablePaymentPlanInstallmentInput Input { get; set; }
    }

    public class DisablePaymentPlanInstallmentInput
    {
        public string CustomerId { get; set; }
        public string PaymentPlanTemplateId { get; set; }
        public string PaymentPlanId { get; set; }

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
