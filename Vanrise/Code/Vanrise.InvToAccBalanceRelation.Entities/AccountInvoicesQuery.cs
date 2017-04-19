using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.InvToAccBalanceRelation.Entities
{
    public class AccountInvoicesQuery
    {
        public Guid AccountTypeId { get; set; }
        public string AccountId { get; set; }
    }
}
