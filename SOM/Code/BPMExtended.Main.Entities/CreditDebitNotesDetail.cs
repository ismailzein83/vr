using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class CreditDebitNotesDetail
    {
        public string DocumentId { get; set; }
        public string DocType { get; set; }
        public string BillingAccountCode { get; set; }
        public string IssueDate { get; set; }
        public string DocumentAmount { get; set; }
        public string DocumentOpenAmount { get; set; }
    }
}
