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
					CustomerMappingSerialized customerMappingSerialized = new CustomerMappingSerialized() { BO = customerMapping.CustomerMapping.BO, CustomerMappingAsString = SerializeCustomerMapping(customerMapping.CustomerMapping) };
					dataManager.WriteRecordToStream(customerMappingSerialized, dbApplyStream);
				}
				object obj = dataManager.FinishDBApplyStream(dbApplyStream);
				dataManager.ApplyCustomerMappingForDB(obj);
			}
		}


		#region CustomerMappingSerialization
		public const char CustomerMappingPropertySeperator = '~';
		public const string CustomerMappingPropertySeperatorAsString = "~";
		public const char CustomerMappingTrunkSeperator = '#';
		public const string CustomerMappingTrunkSeperatorAsString = "#";
		public const char CustomerMappingTrunkPropertySeperator = '$';
		public const string CustomerMappingTrunkPropertySeperatorAsString = "$";

		public static string SerializeCustomerMapping(CustomerMapping customerMapping)
		{
			if (customerMapping == null)
				return null;
			string trunksAsString = null;
			if (customerMapping.InTrunks != null && customerMapping.InTrunks.Count > 0)
			{
				List<string> trunks = new List<string>();
				foreach (var trunk in customerMapping.InTrunks.OrderBy(item => item.TrunkId))
				{
					trunks.Add(string.Format("{1}{0}{2}{0}{3}", CustomerMappingTrunkPropertySeperatorAsString, trunk.TrunkId, trunk.TrunkName, Convert.ToInt32(trunk.TrunkType)));
				}
				trunksAsString = string.Join(CustomerMappingTrunkSeperatorAsString, trunks);
			}
			return string.Format("{1}{0}{2}{0}{3}{0}{4}", CustomerMappingPropertySeperatorAsString, customerMapping.BO, customerMapping.NationalOBA, customerMapping.InternationalOBA, trunksAsString);
		}

		public static CustomerMapping DeserializeCustomerMapping(string serializedCustomerMapping)
		{
			if (string.IsNullOrEmpty(serializedCustomerMapping))
				return null;

			string[] customerMappingPropertiesAsString = serializedCustomerMapping.Split(CustomerMappingPropertySeperator);
			if (customerMappingPropertiesAsString == null || customerMappingPropertiesAsString.Count() != 9)
				return null;

			CustomerMapping customerMapping = new CustomerMapping();
			customerMapping.BO = customerMappingPropertiesAsString[0];
			customerMapping.NationalOBA = customerMappingPropertiesAsString[1];
			customerMapping.InternationalOBA = customerMappingPropertiesAsString[2];
			customerMapping.InTrunks = new List<InTrunk>();

			string trunksAsString = customerMappingPropertiesAsString[3];
			if (!string.IsNullOrEmpty(trunksAsString))
			{
				string[] trunks = trunksAsString.Split(CustomerMappingTrunkSeperator);
				if (trunks != null && trunks.Any())
				{
					foreach (var trunkAsString in trunks)
					{
						if (!string.IsNullOrEmpty(trunkAsString))
							continue;
						string[] trunkProperties = trunkAsString.Split(CustomerMappingTrunkPropertySeperator);
						if (trunkProperties == null || !trunkProperties.Any())
							continue;
						InTrunk trunk = new InTrunk();
						trunk.TrunkId = Guid.Parse(trunkProperties[0]);
						trunk.TrunkName = trunkProperties[1];
						trunk.TrunkType = (TrunkType)int.Parse(trunkProperties[2]);
						customerMapping.InTrunks.Add(trunk);
						/*
							if (trunkAsString != null)
							{
								string[] trunkProperties = trunkAsString.Split(CustomerMappingTrunkPropertySeperator);
								if (trunkProperties != null && trunkProperties.Any())
								{
									InTrunk trunk = new InTrunk();
									trunk.TrunkId = Guid.Parse(trunkProperties[0]);
									trunk.TrunkName = trunkProperties[1];
									trunk.TrunkType = (TrunkType)int.Parse(trunkProperties[2]);
									customerMapping.InTrunks.Add(trunk);
								}
							}
						*/
					}
				}
			}

			if (customerMapping.InTrunks.Count == 0)
				customerMapping.InTrunks = null;

			return customerMapping;
		}

		#endregion
	}
}
