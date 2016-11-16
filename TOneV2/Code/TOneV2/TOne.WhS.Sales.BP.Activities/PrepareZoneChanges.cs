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
		public InArgument<int> OwnerId { get; set; }

		[RequiredArgument]
		public InArgument<SalePriceListOwnerType> OwnerType { get; set; }

		[RequiredArgument]
		public InArgument<IEnumerable<ExistingZone>> ExistingZones { get; set; }

		#endregion

		#region Output Arguments

		[RequiredArgument]
		public OutArgument<IEnumerable<SalePLZoneChange>> SalePLZoneChanges { get; set; }

		#endregion

		protected override void Execute(CodeActivityContext context)
		{
			int ownerId = this.OwnerId.Get(context);
			SalePriceListOwnerType ownerType = this.OwnerType.Get(context);
			IEnumerable<ExistingZone> existingZones = this.ExistingZones.Get(context);

			IEnumerable<SalePLZoneChange> zoneChanges = (ownerType == SalePriceListOwnerType.SellingProduct) ?
				GetSellingProductZoneChanges(ownerId, existingZones) :
				GetCustomerZoneChanges(ownerId, existingZones);

			SalePLZoneChanges.Set(context, zoneChanges);
		}

		#region Private Methods

		private IEnumerable<SalePLZoneChange> GetSellingProductZoneChanges(int sellingProductId, IEnumerable<ExistingZone> existingZones)
		{
			List<SalePLZoneChange> zonesChanges = new List<SalePLZoneChange>();

			IEnumerable<RoutingCustomerInfoDetails> dataByCustomer = GetDataByCustomer(sellingProductId);
			SaleEntityZoneRateLocator futureRateLocator = new SaleEntityZoneRateLocator(new SaleRateReadAllNoCache(dataByCustomer, null, true));

			foreach (ExistingZone existingZone in existingZones)
			{
				if (existingZone.NewRates.Count > 0 || existingZone.ExistingRates.Any(x => x.ChangedRate != null))
				{
					var customerIds = new List<int>();
					foreach (RoutingCustomerInfoDetails customerData in dataByCustomer)
					{
						SaleEntityZoneRate customerRate = futureRateLocator.GetCustomerZoneRate(customerData.CustomerId, sellingProductId, existingZone.ZoneId);
						if (customerRate != null && customerRate.Source == SalePriceListOwnerType.SellingProduct)
							customerIds.Add(customerData.CustomerId);
					}
					zonesChanges.Add(new SalePLZoneChange()
					{
						ZoneName = existingZone.Name,
						CountryId = existingZone.CountryId,
						HasCodeChange = false,
						CustomersHavingRateChange = customerIds
					});
				}
			}

			return zonesChanges;
		}

		private IEnumerable<RoutingCustomerInfoDetails> GetDataByCustomer(int sellingProductId)
		{
			var customerSellingProductManager = new CustomerSellingProductManager();
			IEnumerable<CarrierAccountInfo> customers = customerSellingProductManager.GetCustomersBySellingProductId(sellingProductId);
			if (customers == null)
				return new List<RoutingCustomerInfoDetails>();
			return customers.MapRecords(x => new RoutingCustomerInfoDetails()
			{
				CustomerId = x.CarrierAccountId,
				SellingProductId = sellingProductId
			});
		}

		private IEnumerable<SalePLZoneChange> GetCustomerZoneChanges(int customerId, IEnumerable<ExistingZone> existingZones)
		{
			var zonesChanges = new List<SalePLZoneChange>();
			
			var ownerIds = new List<int>();
			ownerIds.Add(customerId);

			foreach (ExistingZone existingZone in existingZones)
			{
				if (existingZone.NewRates.Count > 0 || existingZone.ExistingRates.Any(x => x.ChangedRate != null))
				{
					var zoneChange = new SalePLZoneChange()
					{
						ZoneName = existingZone.Name,
						CountryId = existingZone.CountryId,
						HasCodeChange = false,
						CustomersHavingRateChange = ownerIds
					};
					zonesChanges.Add(zoneChange);
				}
			}
			return zonesChanges;
		}

		#endregion
	}
}
