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
        public FinalizeEmptyBatchesContext(Action<AccountBalanceType> generateEmptyBatch)
        {
            _generateEmptyBatch = generateEmptyBatch;
        }

        public void GenerateEmptyBatch(AccountBalanceType accountBalanceType)
        {
            _generateEmptyBatch(accountBalanceType);
        }
    }
}
