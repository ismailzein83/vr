using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class AccountUsageToUpdate
    {
        public long AccountUsageId { get; set; }
        public decimal Value { get; set; }
    }
}
