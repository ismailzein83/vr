using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IDWDimensionDataManager : IDataManager 
    {
        List<Dimension> GetDimensions(string tableName);
    }
}
