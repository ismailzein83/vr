using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountBalanceUpdate
    {
        public long AccountId { get; set; }

        public Decimal Value { get; set; }
    }
}
