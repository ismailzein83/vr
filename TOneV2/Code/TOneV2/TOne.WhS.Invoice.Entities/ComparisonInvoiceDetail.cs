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
       public DateTime ToDate { get; set; }
       public DateTime FromDate { get; set; }
       public DateTime IssuedDate { get; set; }
       public DateTime DueDate { get; set; }
       public string SerialNumber { get; set; }
       public String TimeZone { get; set; }
       public String Currency { get; set; }
       public int Calls { get; set; }
       public decimal Duration { get; set; }
       public decimal TotalAmount { get; set; }
       public bool IsLocked { get; set; }
       public bool IsPaid { get; set; }
       public String IssuedBy { get; set; }
       public String PartnerId { get; set; }

    }
}
