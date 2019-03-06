using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.ExcelConversion.Entities;

namespace Retail.Interconnect.Entities
{
    public enum ComparisonSMSCriteria { NbOfSMSs = 0, Amount = 2 }
    public class InvoiceComparisonSMSInput
    {
        public ListMapping ListMapping { get; set; }
        public string DateTimeFormat { get; set; }
        public decimal Threshold { get; set; }
        public Guid InvoiceTypeId { get; set; }
        public Guid InvoiceActionId { get; set; }
        public long InvoiceId { get; set; }
        public long InputFileId { get; set; }
        public List<ComparisonResult> ComparisonResults { get; set; }
        public List<ComparisonSMSCriteria> ComparisonCriterias { get; set; }
        public int? DecimalDigits { get; set; }
    }
}
