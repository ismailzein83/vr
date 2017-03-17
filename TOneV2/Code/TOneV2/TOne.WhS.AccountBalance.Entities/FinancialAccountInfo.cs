using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.AccountBalance.Entities
{
    public class FinancialAccountInfo
    {
        public int FinancialAccountId { get; set; }

        public bool IsCarrierProfile { get; set; }

        public string FinancialAccountName { get; set; }

        public string FinancialAccountDescription { get; set; }
    }

    public enum FinancialAccountEffectiveType { Recent = 1, Current = 2, Future = 3 }
}
