using Vanrise.Data;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IDWCDRDataManager : IDataManager, IBulkApplyDataManager<DWCDR>
    {
        void ApplyDWCDRsToDB(object preparedDWCDRs);
    }
}
