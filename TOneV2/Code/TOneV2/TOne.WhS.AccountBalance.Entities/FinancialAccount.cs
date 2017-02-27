using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.AccountBalance.Entities
{
    public class FinancialAccount
    {
        public int FinancialAccountId { get; set; }

        public int? CarrierProfileId { get; set; }

        public int? CarrierAccountId { get; set; }

        public FinancialAccountSettings Settings { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }
}
