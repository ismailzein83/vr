using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public enum InvoiceField
    {
        CustomField = 0,
        InvoiceId =1,
        Partner = 2,
        SerialNumber = 3,
        FromDate = 4,
        ToDate = 5,
        IssueDate = 6,
        DueDate = 7,
    }
}
