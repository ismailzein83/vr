using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class BillingTransactionQuery
    {
        public List<long> AccountsIds { get; set; }
        public Guid AccountTypeId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime? ToDate { get; set; }

    }
}
