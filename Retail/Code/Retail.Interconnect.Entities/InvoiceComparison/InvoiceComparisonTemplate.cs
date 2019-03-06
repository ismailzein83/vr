using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.ExcelConversion.Entities;

namespace Retail.Interconnect.Entities
{
    public class InvoiceComparisonTemplate
    {
        public long InvoiceComparisonTemplateId { get; set; }
        public Guid InvoiceTypeId { get; set; }
        public string PartnerId { get; set; }
        public InvoiceComparisonTemplateDetails Details { get; set; }
    }
    public class InvoiceComparisonTemplateDetails
    {
        public InvoiceComparisonVoiceTemplate VoiceTemplate { get; set; }
        public InvoiceComparisonSMSTemplate SMSTemplate { get; set; }
    }
    public class InvoiceComparisonVoiceTemplate
    {
        public ListMapping ListMapping { get; set; }
        public string DateTimeFormat { get; set; }
    }
    public class InvoiceComparisonSMSTemplate
    {
        public ListMapping ListMapping { get; set; }
        public string DateTimeFormat { get; set; }
    }
}
