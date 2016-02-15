using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class DWDimensionManager
    {

        public List<DWDimension> GetDimensions(string tableName)
        {
            IDWDimensionDataManager dataManager = FraudDataManagerFactory.GetDataManager<IDWDimensionDataManager>();
            dataManager.TableName = tableName;
            return dataManager.GetDimensions();
        }

    }
}
