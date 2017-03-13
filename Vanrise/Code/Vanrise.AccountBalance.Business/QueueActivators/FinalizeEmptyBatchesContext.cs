using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Business
{
    public class FinalizeEmptyBatchesContext : IFinalizeEmptyBatchesContext
    {
        Action<AccountBalanceType> _generateEmptyBatch;

        public List<AccountBalanceType> FinalizedAccountBalanceTypes { get; set; }

        public FinalizeEmptyBatchesContext(Action<AccountBalanceType> generateEmptyBatch, List<AccountBalanceType> finalizedAccountBalanceTypes)
        {
            _generateEmptyBatch = generateEmptyBatch;
            FinalizedAccountBalanceTypes = finalizedAccountBalanceTypes;
        }

        public void GenerateEmptyBatch(AccountBalanceType accountBalanceType)
        {
            _generateEmptyBatch(accountBalanceType);
        }
    }
}
