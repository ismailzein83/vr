using Vanrise.Data;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IRelatedNumberDataManager : IDataManager, IBulkApplyDataManager<AccountRelatedNumbers>
    {
        void ApplyAccountRelatedNumbersToDB(object preparedAccountRelatedNumbers);

        void SavetoDB(AccountRelatedNumbers record);
    }
}
