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
using Vanrise.Common;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Sales.BP.Activities
{
	public sealed class PrepareZoneChanges : CodeActivity
	{
		#region Input Arguments

		[RequiredArgument]
		public InArgument<IEnumerable<DataByZone>> DataByZone { get; set; }

		[RequiredArgument]
		public InArgument<IEnumerable<NewCustomerCountry>> NewCustomerCountries { get; set; }

		#endregion

		#region Output Arguments

		[RequiredArgument]
		public OutArgument<IEnumerable<SalePLZoneChange>> SalePLZoneChanges { get; set; }

		#endregion

		protected override void Execute(CodeActivityContext context)
		{
			IRatePlanContext ratePlanContext = context.GetRatePlanContext();
			IEnumerable<DataByZone> dataByZone = DataByZone.Get(context);
			IEnumerable<NewCustomerCountry> newCountries = NewCustomerCountries.Get(context);

			IEnumerable<RoutingCustomerInfoDetails> dataByCustomer = GetDataByCustomer(ratePlanContext.OwnerType, ratePlanContext.OwnerId, ratePlanContext.EffectiveDate);
			SaleEntityZoneRateLocator futureRateLocator = new SaleEntityZoneRateLocator(new SaleRateReadAllNoCache(dataByCustomer, null, true));

			IEnumerable<SalePLZoneChange> zoneChanges;

			if (ratePlanContext.OwnerType == SalePriceListOwnerType.SellingProduct)
			{
				zoneChanges = GetSellingProductZoneChanges(dataByZone, dataByCustomer, futureRateLocator);
			}
			else
			{
				RoutingCustomerInfoDetails customerData = dataByCustomer.FirstOrDefault();
				Dictionary<int, IEnumerable<SaleZone>> newCountryZonesByCountry = GetNewCountryZonesByCountry(newCountries, ratePlanContext.OwnerSellingNumberPlanId, ratePlanContext.EffectiveDate);
				zoneChanges = GetCustomerZoneChanges(customerData.CustomerId, customerData.SellingProductId, futureRateLocator, dataByZone, newCountryZonesByCountry);
			}

			SalePLZoneChanges.Set(context, zoneChanges);
		}

		#region Selling Product Methods

		private IEnumerable<SalePLZoneChange> GetSellingProductZoneChanges(IEnumerable<DataByZone> dataByZone, IEnumerable<RoutingCustomerInfoDetails> dataByCustomer, SaleEntityZoneRateLocator futureRateLocator)
		{
			if (dataByZone == null || dataByZone.Count() == 0)
				return null;

			var zoneChanges = new List<SalePLZoneChange>();
			List<int> customerIds;

			foreach (DataByZone zoneData in dataByZone)
			{
				if (DoesZoneHasRateChanges(zoneData))
				{
					customerIds = new List<int>();
					foreach (RoutingCustomerInfoDetails customerData in dataByCustomer)
					{
						SaleEntityZoneRate customerRate = futureRateLocator.GetCustomerZoneRate(customerData.CustomerId, customerData.SellingProductId, zoneData.ZoneId);
						if (customerRate != null && customerRate.Rate != null)
						{
							if (customerRate.Source == SalePriceListOwnerType.SellingProduct || customerRate.Rate.EED.HasValue)
								customerIds.Add(customerData.CustomerId);
						}
					}
					zoneChanges.Add(new SalePLZoneChange()
					{
						ZoneName = zoneData.ZoneName,
						CountryId = zoneData.CountryId,
						HasCodeChange = false,
						CustomersHavingRateChange = customerIds
					});
				}
			}

			return zoneChanges;
		}
		
		#endregion

		#region Customer Methods

		private IEnumerable<SalePLZoneChange> GetCustomerZoneChanges(int customerId, int sellingProductId, SaleEntityZoneRateLocator futureRateLocator, IEnumerable<DataByZone> dataByZone, Dictionary<int, IEnumerable<SaleZone>> newCountryZonesByCountry)
		{
			var zoneChanges = new List<SalePLZoneChange>();

			IEnumerable<int> customerIds = new List<int>() { customerId };
			var zoneChangeIds = new List<long>();

			if (dataByZone != null)
			{
				foreach (DataByZone zoneData in dataByZone)
				{
					if (DoesZoneHasRateChanges(zoneData))
					{
						zoneChangeIds.Add(zoneData.ZoneId);
						var zoneChange = new SalePLZoneChange()
						{
							ZoneName = zoneData.ZoneName,
							CountryId = zoneData.CountryId,
							HasCodeChange = false,
							CustomersHavingRateChange = customerIds
						};
						zoneChanges.Add(zoneChange);
					}
				}
			}

			if (newCountryZonesByCountry != null)
			{
				foreach (IEnumerable<SaleZone> countryZones in newCountryZonesByCountry.Values)
				{
					foreach (SaleZone countryZone in countryZones)
					{
						if (zoneChangeIds.Contains(countryZone.SaleZoneId))
							continue;
						SaleEntityZoneRate zoneRate = futureRateLocator.GetCustomerZoneRate(customerId, sellingProductId, countryZone.SaleZoneId);
						if (zoneRate == null || zoneRate.Rate == null || zoneRate.Source != SalePriceListOwnerType.SellingProduct)
							continue;
						zoneChanges.Add(new SalePLZoneChange()
						{
							ZoneName = countryZone.Name,
							CountryId = countryZone.CountryId,
							HasCodeChange = false,
							CustomersHavingRateChange = customerIds
						});
					}
				}
			}

			return zoneChanges;
		}

		private Dictionary<int, IEnumerable<SaleZone>> GetNewCountryZonesByCountry(IEnumerable<NewCustomerCountry> newCountries, int sellingNumberPlanId, DateTime effectiveOn)
		{
			if (newCountries == null || newCountries.Count() == 0)
				return null;

			var newCountryZonesByCountry = new Dictionary<int, IEnumerable<SaleZone>>();

			IEnumerable<int> newCountryIds = newCountries.MapRecords(x => x.CountryId);
			var saleZoneManager = new SaleZoneManager();

			foreach (int newCountryId in newCountryIds)
			{
				IEnumerable<SaleZone> countryZones = saleZoneManager.GetSaleZonesByCountryId(sellingNumberPlanId, newCountryId, effectiveOn);
				if (countryZones == null || countryZones.Count() == 0)
					continue;
				if (!newCountryZonesByCountry.ContainsKey(newCountryId))
					newCountryZonesByCountry.Add(newCountryId, countryZones);
			}

			return newCountryZonesByCountry;
		}

		#endregion

		#region Common Methods

		private IEnumerable<RoutingCustomerInfoDetails> GetDataByCustomer(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveDate)
		{
			var customerIds = new List<int>();
			int sellingProductId;

			if (ownerType == SalePriceListOwnerType.SellingProduct)
			{
				IEnumerable<int> customerIdsAssignedToSellingProduct = new CustomerSellingProductManager().GetCustomerIdsAssignedToSellingProduct(ownerId, effectiveDate);

				if (customerIdsAssignedToSellingProduct == null || customerIdsAssignedToSellingProduct.Count() == 0)
					return null;

				customerIds.AddRange(customerIdsAssignedToSellingProduct);
				sellingProductId = ownerId;
			}
			else
			{
				customerIds.Add(ownerId);

				int? effectiveSellingProductId = new CustomerSellingProductManager().GetEffectiveSellingProductId(ownerId, effectiveDate, false);
				if (!effectiveSellingProductId.HasValue)
					throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Customer '{0}' is not assigned to a Selling Product", ownerId));

				sellingProductId = effectiveSellingProductId.Value;
			}

			return customerIds.MapRecords(customerId => new RoutingCustomerInfoDetails()
			{
				CustomerId = customerId,
				SellingProductId = sellingProductId
			});
		}

		private bool DoesZoneHasRateChanges(DataByZone zoneData)
		{
			return (zoneData.NormalRateToChange != null || zoneData.NormalRateToClose != null || (zoneData.OtherRatesToChange != null && zoneData.OtherRatesToChange.Count > 0) || (zoneData.OtherRatesToClose != null && zoneData.OtherRatesToClose.Count > 0));
		}
		
		#endregion
	}
}
