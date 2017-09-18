using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities.DisputeCase
{
    public class DisputeCase
    {
        public int DisputeCaseId { get; set; }
        public string CaseNumber { get; set; }
        public string PartnerName { get; set; }
        public string PartnerType { get; set; }
        public string InvoiceNo { get; set; }
        public string Status { get; set; }
        public int StatusCode { get; set; }
        public DateTime CreatedTime { get; set; }
        public string CreatedBy { get; set; }

    }
}
