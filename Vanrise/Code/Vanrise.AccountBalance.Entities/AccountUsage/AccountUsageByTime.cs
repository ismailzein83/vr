using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class AccountUsageByTime
    {
        public string AccountId { get; set; }
        public DateTime EndPeriod { get; set; }
    }
}
