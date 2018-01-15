using System.Collections.Generic;
using TOne.WhS.Routing.Entities;
using Vanrise.Data;

namespace TOne.WhS.Routing.Data
{
    public interface IRPQualityConfigurationDataManager : IDataManager, IBulkApplyDataManager<RPQualityConfigurationData>, IRoutingDataManager
    {
        void ApplyQualityConfigurationsToDB(object qualityConfigurations);

        IEnumerable<RPQualityConfigurationData> GetRPQualityConfigurationData(); 
    }
}