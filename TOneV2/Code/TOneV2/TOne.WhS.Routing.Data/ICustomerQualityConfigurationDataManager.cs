using System.Collections.Generic;
using TOne.WhS.Routing.Entities;
using Vanrise.Data;

namespace TOne.WhS.Routing.Data
{
    public interface ICustomerQualityConfigurationDataManager : IDataManager, IBulkApplyDataManager<CustomerRouteQualityConfigurationData>, IRoutingDataManager
    {
        void ApplyQualityConfigurationsToDB(object qualityConfigurations);

        IEnumerable<CustomerRouteQualityConfigurationData> GetCustomerRouteQualityConfigurationsData();

        void UpdateCustomerRouteQualityConfigurationsData(List<CustomerRouteQualityConfigurationData> customerRouteQualityConfigurationData);

        List<CustomerRouteQualityConfigurationData> GetCustomerRouteQualityConfigurationsDataAfterVersionNumber(int versionNumber);
    }
}
