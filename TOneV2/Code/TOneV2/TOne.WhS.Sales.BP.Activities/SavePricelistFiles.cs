using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;

namespace TOne.WhS.Sales.BP.Activities
{
	public class SavePricelistFiles : CodeActivity
	{
		#region Input Arguments

		[RequiredArgument]
		public InArgument<IEnumerable<SalePLZoneChange>> SalePLZoneChanges { get; set; }

		[RequiredArgument]
		public InArgument<IEnumerable<CustomerCountryToChange>> CustomerCountriesToChange { get; set; }

		#endregion

		#region Output Arguments

		[RequiredArgument]
		public OutArgument<IEnumerable<int>> CustomerIdsWithPriceList { get; set; }

		#endregion

		protected override void Execute(CodeActivityContext context)
		{
			IRatePlanContext ratePlanContext = context.GetRatePlanContext();
			IEnumerable<SalePLZoneChange> zoneChanges = SalePLZoneChanges.Get(context);
			IEnumerable<CustomerCountryToChange> countriesToChange = CustomerCountriesToChange.Get(context);

			IEnumerable<int> customerIdsWithPriceList;
			SalePLChangeType plChangeType;
			IEnumerable<int> endedCountryIds = null;
			DateTime? countriesEndedOn = null;

			if (countriesToChange != null && countriesToChange.Count() > 0)
			{
				customerIdsWithPriceList = new List<int>() { ratePlanContext.OwnerId };
				plChangeType = SalePLChangeType.CountryAndRate;
				endedCountryIds = countriesToChange.MapRecords(x => x.CountryId);
				countriesEndedOn = countriesToChange.First().CloseEffectiveDate;
			}
			else
			{
				customerIdsWithPriceList = GetCustomerIdsWithPriceList(zoneChanges);
				plChangeType = SalePLChangeType.Rate;
			}

			var salePricelistFileContext = new SalePricelistFileContext
			{
				SellingNumberPlanId = ratePlanContext.OwnerSellingNumberPlanId,
				ProcessInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID,
				CustomerIds = customerIdsWithPriceList,
				ZoneChanges = zoneChanges,
				EffectiveDate = ratePlanContext.EffectiveDate,
				ChangeType = plChangeType,
				EndedCountryIds = endedCountryIds,
				CountriesEndedOn = countriesEndedOn
			};

			var salePricelistManager = new SalePriceListManager();
			salePricelistManager.SavePricelistFiles(salePricelistFileContext);

			CustomerIdsWithPriceList.Set(context, customerIdsWithPriceList);
		}

		#region Private Methods

		private IEnumerable<int> GetCustomerIdsWithPriceList(IEnumerable<SalePLZoneChange> zonesChanges)
		{
			if (zonesChanges == null || zonesChanges.Count() == 0)
				return null;

			HashSet<int> countryIds;
			HashSet<int> customerIds;
			SetCountryAndCustomerIds(zonesChanges, out countryIds, out customerIds);

			if (customerIds == null || customerIds.Count == 0)
				return null;

			var customerIdsWithPriceList = new List<int>();
			var customerCountryManager = new CustomerCountryManager();

			foreach (int customerId in customerIds)
			{
				IEnumerable<int> soldCountryIds = customerCountryManager.GetCountryIdsEffectiveAfterByCustomer(customerId, DateTime.Today);

				if (soldCountryIds != null && soldCountryIds.Any(soldCountryId => countryIds.Contains(soldCountryId)))
					customerIdsWithPriceList.Add(customerId);
			}

			return customerIdsWithPriceList;
		}

		private void SetCountryAndCustomerIds(IEnumerable<SalePLZoneChange> zoneChanges, out HashSet<int> countryIds, out HashSet<int> customerIds)
		{
			countryIds = new HashSet<int>(zoneChanges.MapRecords(x => x.CountryId));
			IEnumerable<int> customerIdList = zoneChanges.SelectMany(x => x.CustomersHavingRateChange);
			customerIds = (customerIdList != null) ? new HashSet<int>(customerIdList) : null;
		}

		#endregion
	}
}
