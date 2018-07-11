using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class Invoice
    {
        public long ServiceId { get; set; }
        public string SerialNumber { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime IssueDate { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }

    }
}
