using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.InvToAccBalanceRelation.Entities
{
    public class AccountInvoice
    {
        public long InvoiceId { get; set; }
        public string SerialNumber { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime IssueDate { get; set; }
        public string Note { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }
        public int CurrencyId { get; set; }
    }
}
