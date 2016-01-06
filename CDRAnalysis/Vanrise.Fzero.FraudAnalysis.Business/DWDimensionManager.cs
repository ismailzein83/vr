using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class DWDimensionManager
    {

        public List<Dimension> GetDimensions(string tableName)
        {
            IDWDimensionDataManager dataManager = FraudDataManagerFactory.GetDataManager<IDWDimensionDataManager>();

            return dataManager.GetDimensions(tableName);
        }

    }
}
