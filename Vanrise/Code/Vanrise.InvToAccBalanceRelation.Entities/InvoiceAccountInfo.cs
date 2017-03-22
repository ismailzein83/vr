using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.InvToAccBalanceRelation.Entities
{
    public class InvoiceAccountInfo
    {
        public Guid InvoiceTypeId { get; set; }

        public string PartnerId { get; set; }
    }
}
