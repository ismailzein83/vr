
namespace Vanrise.AccountBalance.Business
{
    public interface IFinalizeEmptyBatchesContext
    {
        void GenerateEmptyBatch(AccountBalanceType accountBalanceType);
    }
}
