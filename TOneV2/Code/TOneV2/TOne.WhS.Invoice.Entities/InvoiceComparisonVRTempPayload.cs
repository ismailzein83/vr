using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.Invoice.Entities
{
    public class InvoiceComparisonVRTempPayload : VRTempPayloadSettings
    {
        public bool IsCustomer { get; set; }
        public int FinancialAccountId { get; set; }
        public long InvoiceId { get; set; }
        public InvoiceComparisonVoiceInput VoiceInput { get; set; }
        public InvoiceComparisonSMSInput SMSInput { get; set; }
    }
}
