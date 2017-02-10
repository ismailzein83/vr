using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class BillingTransactionQuery
    {
        public List<String> AccountsIds { get; set; }

        public List<Guid> TransactionTypeIds { get; set; }
        public Guid AccountTypeId { get; set; }

        public DateTime FromTime { get; set; }

        public DateTime? ToTime { get; set; }

    }
}
