using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class AccountStatementQuery
    {
        public long AccountId { get; set; }
        public Guid AccountTypeId { get; set; }
        public DateTime FromDate { get; set; }
    }
}
