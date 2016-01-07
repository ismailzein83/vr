using System.Collections.Generic;
using Vanrise.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IDWDimensionDataManager : IDataManager, IBulkApplyDataManager<DWDimension>
    {
        List<DWDimension> GetDimensions();

        void ApplyDWDimensionsToDB(object preparedDWDimensions);

        void SaveDWDimensionsToDB(List<DWDimension> dwDimensions);

        string TableName { set; }
    }
}
