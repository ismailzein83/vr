using System.Collections.Generic;

namespace Vanrise.AccountBalance.Business
{
    public interface IFinalizeEmptyBatchesContext
    {
        List<AccountBalanceType> FinalizedAccountBalanceTypes { get; }

        void GenerateEmptyBatch(AccountBalanceType accountBalanceType);
    }
}
