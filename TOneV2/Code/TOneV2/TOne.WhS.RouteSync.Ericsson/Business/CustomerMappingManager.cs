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

			var customerMappings = (carrierMappings != null) ? carrierMappings.FindAllRecords(item => item.CustomerMapping != null) : null;

			if (customerMappings != null && customerMappings.Any())
			{
				object dbApplyStream = dataManager.InitialiazeStreamForDBApply();
				foreach (var customerMapping in customerMappings)
				{
					CustomerMappingSerialized customerMappingSerialized = new CustomerMappingSerialized() { BO = customerMapping.CustomerMapping.BO, CustomerMappingAsString = Helper.SerializeCustomerMapping(customerMapping.CustomerMapping) };
					dataManager.WriteRecordToStream(customerMappingSerialized, dbApplyStream);
				}
				object obj = dataManager.FinishDBApplyStream(dbApplyStream);
				dataManager.ApplyCustomerMappingForDB(obj);
			}
		}
	}
}
