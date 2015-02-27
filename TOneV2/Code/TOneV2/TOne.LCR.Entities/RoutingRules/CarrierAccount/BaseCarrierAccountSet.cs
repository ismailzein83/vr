using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public abstract class BaseCarrierAccountSet
    {
        public virtual CarrierAccountSetMatch GetMatch()
        {
            return null;
        }

        public virtual bool IsAccountIdIncluded(string accountId)
        {
            return true;
        }
    }

    public class CarrierAccountSetMatch
    {
        public bool IsMatchingAllAccounts { get; set; }

        public List<string> MatchAccountIds { get; set; }
    }
}
