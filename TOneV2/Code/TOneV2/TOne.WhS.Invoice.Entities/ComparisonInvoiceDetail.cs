using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Invoice.Entities
{
    public class ComparisonInvoiceDetail
    {
       public String To { get; set; }
       public DateTime toDate { get; set; }
       public DateTime fromDate { get; set; }
       public DateTime issuedDate { get; set; }
       public DateTime dueDate { get; set; }
       public string serialNumber { get; set; }
       public String timeZone { get; set; }
       public String currency { get; set; }
       public int calls { get; set; }
       public decimal duration { get; set; }
       public decimal totalAmount { get; set; }
       public bool isLocked { get; set; }
       public bool isPaid { get; set; }
       public String issuedBy { get; set; }
       public String PartnerId { get; set; }

    }
}
