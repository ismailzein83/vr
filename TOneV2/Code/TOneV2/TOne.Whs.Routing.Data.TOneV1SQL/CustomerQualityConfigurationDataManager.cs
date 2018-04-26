using System;
using System.Collections.Generic;
using System.Data;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.Data.SQL;

namespace TOne.Whs.Routing.Data.TOneV1SQL
{
    public class CustomerQualityConfigurationDataManager : RoutingDataManager, ICustomerQualityConfigurationDataManager
    {
        public void ApplyQualityConfigurationsToDB(object qualityConfigurations)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CustomerRouteQualityConfigurationData> GetCustomerRouteQualityConfigurationsData()
        {
            throw new NotImplementedException();
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            throw new NotImplementedException();
        }

        public object InitialiazeStreamForDBApply()
        {
            throw new NotImplementedException();
        }

        public void WriteRecordToStream(CustomerRouteQualityConfigurationData record, object dbApplyStream)
        {
            throw new NotImplementedException();
        }
    }
}