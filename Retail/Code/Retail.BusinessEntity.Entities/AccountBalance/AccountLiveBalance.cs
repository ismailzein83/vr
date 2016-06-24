using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountLiveBalance
    {
        public long AccountId { get; set; }

        public Decimal UsageBalance { get; set; }

        public Decimal CurrentBalance { get; set; }
    }
}
