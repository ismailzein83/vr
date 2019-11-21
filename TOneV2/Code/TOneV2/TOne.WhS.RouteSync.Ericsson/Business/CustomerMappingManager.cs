using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using TOne.WhS.RouteSync.Ericsson.Data;

namespace TOne.WhS.RouteSync.Ericsson.Business
{
    public class CustomerMappingManager
    {
        public void Initialize(string switchId, IEnumerable<CarrierMapping> carrierMappings)
        {
            ICustomerMappingDataManager dataManager = RouteSyncEricssonDataManagerFactory.GetDataManager<ICustomerMappingDataManager>();
            dataManager.SwitchId = switchId;

            dataManager.Initialize(new CustomerMappingInitializeContext());

            if (carrierMappings == null && !carrierMappings.Any())
                return;

            object dbApplyStream = dataManager.InitialiazeStreamForDBApply();
            foreach (var carrierMapping in carrierMappings)
            {
                if (carrierMapping.CustomerMapping != null && carrierMapping.CustomerMapping.BO.HasValue)
                {
                    CustomerMappingSerialized customerMappingSerialized = new CustomerMappingSerialized() { BO = carrierMapping.CustomerMapping.BO.Value, CustomerMappingAsString = Helper.SerializeCustomerMapping(carrierMapping.CustomerMapping) };
                    dataManager.WriteRecordToStream(customerMappingSerialized, dbApplyStream);
                }
            }
            object obj = dataManager.FinishDBApplyStream(dbApplyStream);
            dataManager.ApplyCustomerMappingForDB(obj);
        }
    }
}