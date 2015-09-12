using Vanrise.Data;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IRelatedNumberDataManager : IDataManager
    {
        void SavetoDB(AccountRelatedNumbers record);
    }
}
