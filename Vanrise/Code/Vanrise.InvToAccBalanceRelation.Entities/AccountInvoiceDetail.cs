using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.InvToAccBalanceRelation.Entities
{
    public class AccountInvoiceDetail
    {
        public AccountInvoice Entity { get; set; }
        public string CurrencyName { get; set; }
    }
}
