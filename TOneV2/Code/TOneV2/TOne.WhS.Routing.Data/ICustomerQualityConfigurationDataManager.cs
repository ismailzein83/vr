using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using Vanrise.Data;

namespace TOne.WhS.Routing.Data
{
    public interface ICustomerQualityConfigurationDataManager : IDataManager, IBulkApplyDataManager<CustomerRouteQualityConfigurationData>, IRoutingDataManager
    {
        void ApplyQualityConfigurationsToDB(object qualityConfigurations);

        IEnumerable<CustomerRouteQualityConfigurationData> GetCustomerRouteQualityConfigurationsData(); 
    }
}
