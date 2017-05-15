using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class UpdateAccountBalanceTypeCombination
    {
        public Guid BalanceAccountTypeId { get; set; }

        public List<Guid> TransactionTypeIds { get; set; }
    }
}
