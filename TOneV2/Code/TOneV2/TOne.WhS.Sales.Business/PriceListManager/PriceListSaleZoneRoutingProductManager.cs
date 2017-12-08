using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Business
{
    public class PriceListSaleZoneRoutingProductManager
	{
		public void ProcessZoneRoutingProducts(IProcessSaleZoneRoutingProductsContext context)
		{
            var newSaleZoneRoutingProducts = new List<NewSaleZoneRoutingProduct>();
            Process(context.SaleZoneRoutingProductsToAdd, context.SaleZoneRoutingProductsToClose, context.ExistingSaleZoneRoutingProducts, context.ExistingZones, context.ExplicitlyChangedExistingCustomerCountries, newSaleZoneRoutingProducts, context.OwnerId);

            List<NewSaleZoneRoutingProduct> newRoutingProducts = new List<NewSaleZoneRoutingProduct>();
            newRoutingProducts.AddRange(context.SaleZoneRoutingProductsToAdd.SelectMany(x => x.NewSaleZoneRoutingProducts));
            newRoutingProducts.AddRange(newSaleZoneRoutingProducts);
            context.NewSaleZoneRoutingProducts = newRoutingProducts;
			context.ChangedSaleZoneRoutingProducts = context.ExistingSaleZoneRoutingProducts.Where(x => x.ChangedSaleZoneRoutingProduct != null).Select(x => x.ChangedSaleZoneRoutingProduct);
		}

		#region Private Methods

        private void Process(IEnumerable<SaleZoneRoutingProductToAdd> routingProductsToAdd, IEnumerable<SaleZoneRoutingProductToClose> routingProductsToClose, IEnumerable<ExistingSaleZoneRoutingProduct> existingRoutingProducts, IEnumerable<ExistingZone> existingZones, IEnumerable<ExistingCustomerCountry> explicitlyChangedExistingCustomerCountries, List<NewSaleZoneRoutingProduct> newSaleZoneRoutingProducts,int ownerId)
		{
			Dictionary<int, List<ExistingZone>> existingZonesByCountry;
			ExistingZonesByName existingZonesByName;
			StructureExistingZonesByCountryAndName(existingZones, out existingZonesByCountry, out existingZonesByName);

			ExistingSaleZoneRoutingProductsByZoneName existingRoutingProductsByZoneName = StructureExistingRoutingProductsByZoneName(existingRoutingProducts);

			foreach (SaleZoneRoutingProductToAdd routingProductToAdd in routingProductsToAdd)
			{
				List<ExistingSaleZoneRoutingProduct> matchedExistingRoutingProducts;
				if (existingRoutingProductsByZoneName.TryGetValue(routingProductToAdd.ZoneName, out matchedExistingRoutingProducts))
				{
					CloseOverlappedExistingRoutingProducts(routingProductToAdd, matchedExistingRoutingProducts);
				}
				ProcessRoutingProductToAdd(routingProductToAdd, existingZonesByName);
			}

			foreach (SaleZoneRoutingProductToClose routingProductToClose in routingProductsToClose)
			{
				List<ExistingSaleZoneRoutingProduct> matchedExistingRoutingProduct;
				if (existingRoutingProductsByZoneName.TryGetValue(routingProductToClose.ZoneName, out matchedExistingRoutingProduct))
				{
					CloseExistingRoutingProducts(routingProductToClose, matchedExistingRoutingProduct);
				}
			}
            if (explicitlyChangedExistingCustomerCountries.Count() > 0)
            {
                
                Dictionary<int, CountryRange> endedCountryRangesByCountryId = GetEndedCountryRangesByCountryId(explicitlyChangedExistingCustomerCountries);
                IEnumerable<int> customerIds = CreateListFromItem<int>(ownerId);
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                int sellingProductId = carrierAccountManager.GetSellingProductId(ownerId);
                List<int> sellingProductIds = CreateListFromItem<int>(sellingProductId); 
                int sellingNumberPlanId = carrierAccountManager.GetSellingNumberPlanId(ownerId);
                SaleZoneManager saleZoneManager = new SaleZoneManager();
                IEnumerable<long> zoneIds = saleZoneManager.GetEffectiveSaleZonesByCountryIds(sellingNumberPlanId, endedCountryRangesByCountryId.Keys, DateTime.Today, true).Select(z => z.SaleZoneId);
                CustomerZoneRoutingProductHistoryReader customerZoneRoutingProductHistoryReader = new CustomerZoneRoutingProductHistoryReader(customerIds, sellingProductIds, zoneIds);
                CustomerZoneRoutingProductHistoryLocator customerZoneRoutingProductHistoryLocator = new CustomerZoneRoutingProductHistoryLocator(customerZoneRoutingProductHistoryReader);
                var countryManager = new Vanrise.Common.Business.CountryManager();

                foreach (ExistingCustomerCountry changedExistingCountry in explicitlyChangedExistingCustomerCountries)
                {
                    int countryId = changedExistingCountry.CustomerCountryEntity.CountryId;
                    string countryName = countryManager.GetCountryName(countryId);

                    List<ExistingZone> matchedExistingZones = existingZonesByCountry.GetRecord(countryId);
                    if (matchedExistingZones == null || matchedExistingZones.Count == 0)
                        throw new DataIntegrityValidationException(string.Format("No existing zones for country '{0}' were found", countryName));

                    CountryRange countryRange = endedCountryRangesByCountryId.GetRecord(countryId);
                    if (countryRange == null)
                        throw new DataIntegrityValidationException(string.Format("The BED of country '{0}' was not found", countryName));

                    ProcessChangedExistingCountry(changedExistingCountry, matchedExistingZones, countryRange, customerZoneRoutingProductHistoryLocator, newSaleZoneRoutingProducts,ownerId,sellingProductId);
                }
            }
		}
        private List<T> CreateListFromItem<T>(T item)
        {
            return new List<T>() { item };
        }
		private void StructureExistingZonesByCountryAndName(IEnumerable<ExistingZone> existingZones, out Dictionary<int, List<ExistingZone>> existingZonesByCountry, out ExistingZonesByName existingZonesByName)
		{
			existingZonesByCountry = new Dictionary<int, List<ExistingZone>>();
			existingZonesByName = new ExistingZonesByName();

			List<ExistingZone> zones;

			foreach (ExistingZone existingZone in existingZones)
			{
				if (!existingZonesByCountry.TryGetValue(existingZone.CountryId, out zones))
				{
					zones = new List<ExistingZone>();
					existingZonesByCountry.Add(existingZone.CountryId, zones);
				}
				zones.Add(existingZone);

				if (!existingZonesByName.TryGetValue(existingZone.Name, out zones))
				{
					zones = new List<ExistingZone>();
					existingZonesByName.Add(existingZone.Name, zones);
				}
				zones.Add(existingZone);
			}
		}
		private ExistingSaleZoneRoutingProductsByZoneName StructureExistingRoutingProductsByZoneName(IEnumerable<ExistingSaleZoneRoutingProduct> existingRoutingProducts)
		{
			var routingProductsByZoneName = new ExistingSaleZoneRoutingProductsByZoneName();

			if (existingRoutingProducts == null)
				return routingProductsByZoneName;

			List<ExistingSaleZoneRoutingProduct> routingProductList = null;
			var saleZoneManager = new SaleZoneManager();

			foreach (ExistingSaleZoneRoutingProduct routingProduct in existingRoutingProducts)
			{
				string zoneName = saleZoneManager.GetSaleZoneName(routingProduct.SaleZoneRoutingProductEntity.SaleZoneId);

				if (!routingProductsByZoneName.TryGetValue(zoneName, out routingProductList))
				{
					routingProductList = new List<ExistingSaleZoneRoutingProduct>();
					routingProductsByZoneName.Add(zoneName, routingProductList);
				}

				routingProductList.Add(routingProduct);
			}

			return routingProductsByZoneName;
		}

		private void CloseOverlappedExistingRoutingProducts(SaleZoneRoutingProductToAdd routingProductToAdd, IEnumerable<ExistingSaleZoneRoutingProduct> matchedExistingRoutingProducts)
		{
			foreach (ExistingSaleZoneRoutingProduct existingRoutingProduct in matchedExistingRoutingProducts)
			{
				if (existingRoutingProduct.IsOverlappedWith(routingProductToAdd))
				{
					DateTime changedSaleZoneRoutingProductEED = Utilities.Max(existingRoutingProduct.BED, routingProductToAdd.BED);
					existingRoutingProduct.ChangedSaleZoneRoutingProduct = new ChangedSaleZoneRoutingProduct()
					{
						SaleEntityRoutingProductId = existingRoutingProduct.SaleZoneRoutingProductEntity.SaleEntityRoutingProductId,
						EED = changedSaleZoneRoutingProductEED
					};
					routingProductToAdd.ChangedExistingSaleZoneRoutingProducts.Add(existingRoutingProduct);
				}
			}
		}
		private void ProcessRoutingProductToAdd(SaleZoneRoutingProductToAdd routingProductToAdd, ExistingZonesByName existingZonesByName)
		{
			List<ExistingZone> matchedExistingZones;
			existingZonesByName.TryGetValue(routingProductToAdd.ZoneName, out matchedExistingZones);

			DateTime newSaleZoneRoutingProductBED = routingProductToAdd.BED;
			bool shouldAddNewSaleZoneRoutingProducts = true;

			foreach (var existingZone in matchedExistingZones.OrderBy(x => x.BED))
			{
				if (existingZone.EED.VRGreaterThan(existingZone.BED) && existingZone.EED.VRGreaterThan(newSaleZoneRoutingProductBED) && routingProductToAdd.EED.VRGreaterThan(existingZone.BED))
				{
					AddNewSaleZoneRoutingProduct(routingProductToAdd, ref newSaleZoneRoutingProductBED, existingZone, out shouldAddNewSaleZoneRoutingProducts);
					if (!shouldAddNewSaleZoneRoutingProducts)
						break;
				}
			}
		}
		private void AddNewSaleZoneRoutingProduct(SaleZoneRoutingProductToAdd saleZoneRoutingProductToAdd, ref DateTime newSaleZoneRoutingProductBED, ExistingZone existingZone, out bool shouldAddNewSaleZoneRoutingProducts)
		{
			shouldAddNewSaleZoneRoutingProducts = false;

			var newSaleZoneRoutingProduct = new NewSaleZoneRoutingProduct
			{
				RoutingProductId = saleZoneRoutingProductToAdd.ZoneRoutingProductId,
				SaleZoneId = saleZoneRoutingProductToAdd.ZoneId,
				BED = Utilities.Max(existingZone.BED, newSaleZoneRoutingProductBED),
				EED = saleZoneRoutingProductToAdd.EED
			};

			if (newSaleZoneRoutingProduct.EED.VRGreaterThan(existingZone.EED)) // => existingZone.EED != null
			{
				newSaleZoneRoutingProduct.EED = existingZone.EED;
				newSaleZoneRoutingProductBED = newSaleZoneRoutingProduct.EED.Value;
				shouldAddNewSaleZoneRoutingProducts = true;
			}

			//existingZone.NewSaleZoneRoutingProducts.Add(newSaleZoneRoutingProduct);
			saleZoneRoutingProductToAdd.NewSaleZoneRoutingProducts.Add(newSaleZoneRoutingProduct);
		}

		private void CloseExistingRoutingProducts(SaleZoneRoutingProductToClose routingProductToClose, IEnumerable<ExistingSaleZoneRoutingProduct> matchedExistingRoutingProducts)
		{
			foreach (ExistingSaleZoneRoutingProduct existingRoutingProduct in matchedExistingRoutingProducts)
			{
				if (existingRoutingProduct.EED.VRGreaterThan(routingProductToClose.CloseEffectiveDate))
				{
					existingRoutingProduct.ChangedSaleZoneRoutingProduct = new ChangedSaleZoneRoutingProduct()
					{
						SaleEntityRoutingProductId = existingRoutingProduct.SaleZoneRoutingProductEntity.SaleEntityRoutingProductId,
						EED = Utilities.Max(existingRoutingProduct.BED, routingProductToClose.CloseEffectiveDate)
					};
					routingProductToClose.ChangedExistingSaleZoneRoutingProducts.Add(existingRoutingProduct);
				}
			}
		}

        private void ProcessChangedExistingCountry(ExistingCustomerCountry changedExistingCountry, IEnumerable<ExistingZone> matchedExistingZones, CountryRange countryRange, CustomerZoneRoutingProductHistoryLocator customerZoneRoutingProductHistoryLocator, List<NewSaleZoneRoutingProduct> newSaleZoneRoutingProducts,int ownerId,int sellingproductId)
		{
			foreach (ExistingZone existingZone in matchedExistingZones)
			{
				foreach (ExistingSaleZoneRoutingProduct existingZoneRP in existingZone.ExistingZoneRoutingProducts)
				{
					if (existingZoneRP.EED.VRGreaterThan(changedExistingCountry.ChangedCustomerCountry.EED))
					{
						existingZoneRP.ChangedSaleZoneRoutingProduct = new ChangedSaleZoneRoutingProduct()
						{
							SaleEntityRoutingProductId = existingZoneRP.SaleZoneRoutingProductEntity.SaleEntityRoutingProductId,
							EED = Vanrise.Common.Utilities.Max(existingZoneRP.BED, changedExistingCountry.ChangedCustomerCountry.EED)
						};
					}

				}
                if (countryRange.EED.VRGreaterThan(countryRange.BED) && existingZone.BED < changedExistingCountry.EED.Value)
                    AddZoneRoutingProducts(existingZone, customerZoneRoutingProductHistoryLocator, countryRange, newSaleZoneRoutingProducts, ownerId, sellingproductId);
			}
		}

        private void AddZoneRoutingProducts(ExistingZone existingZone, CustomerZoneRoutingProductHistoryLocator customerZoneRoutingProductHistoryLocator, CountryRange countryRange, List<NewSaleZoneRoutingProduct> newSaleZoneRoutingProducts,int ownerId, int sellingProductId)
        {
            List<SaleEntityZoneRoutingProductHistoryRecord> customerZoneRoutingProductHistory = customerZoneRoutingProductHistoryLocator.GetCustomerZoneRoutingProductHistory(ownerId, sellingProductId, existingZone.Name, existingZone.CountryId).ToList();
            Action<SaleEntityZoneRoutingProductHistoryRecord, ZoneRoutingProduct> mapSaleZoneroutingProduct = (saleEntityZoneRoutingProductHistoryRecord, zoneRoutingProduct) =>
            {
                zoneRoutingProduct.OwnerId = ownerId;
                zoneRoutingProduct.RoutingProductId = saleEntityZoneRoutingProductHistoryRecord.RoutingProductId;
                zoneRoutingProduct.BED = saleEntityZoneRoutingProductHistoryRecord.BED;
                zoneRoutingProduct.EED = saleEntityZoneRoutingProductHistoryRecord.EED;
                zoneRoutingProduct.SaleZoneId = existingZone.ZoneId;
                zoneRoutingProduct.Source = saleEntityZoneRoutingProductHistoryRecord.Source;
            };
            var countryRangeAsList = new List<CountryRange>() { countryRange };
            IEnumerable<ZoneRoutingProduct> ZoneRoutingProducts = Utilities.GetQIntersectT<CountryRange, SaleEntityZoneRoutingProductHistoryRecord, ZoneRoutingProduct>(countryRangeAsList, customerZoneRoutingProductHistory, mapSaleZoneroutingProduct);

            foreach (ZoneRoutingProduct zoneRoutingProduct in ZoneRoutingProducts)
            {
                if (zoneRoutingProduct.Source != SaleEntityZoneRoutingProductSource.CustomerZone)
                {
                    newSaleZoneRoutingProducts.Add(new NewSaleZoneRoutingProduct()
                    {
                        RoutingProductId = zoneRoutingProduct.RoutingProductId,
                        SaleZoneId = zoneRoutingProduct.SaleZoneId,
                        BED = zoneRoutingProduct.BED,
                        EED = zoneRoutingProduct.EED
                    });
                }
            }
        }
        private class ZoneRoutingProduct : Vanrise.Entities.IDateEffectiveSettings, Vanrise.Entities.IDateEffectiveSettingsEditable
        {
            public int OwnerId { get; set; }

            public long SaleZoneId { get; set; }

            public int RoutingProductId { get; set; }

            public DateTime BED { get; set; }

            public DateTime? EED { get; set; }
            public SaleEntityZoneRoutingProductSource Source { get; set; }
        }
        private Dictionary<int, CountryRange> GetEndedCountryRangesByCountryId(IEnumerable<ExistingCustomerCountry> changedExistingCountries)
        {
            var endedCountryRangesByCountryId = new Dictionary<int, CountryRange>();

            foreach (ExistingCustomerCountry endedCountry in changedExistingCountries)
            {
                int endedCountryId = endedCountry.CustomerCountryEntity.CountryId;

                if (!endedCountryRangesByCountryId.ContainsKey(endedCountryId))
                {
                    endedCountryRangesByCountryId.Add(endedCountryId, new CountryRange()
                    {
                        BED = Utilities.Max(endedCountry.BED, DateTime.Today),
                        EED = endedCountry.EED
                    });
                }
            }

            return endedCountryRangesByCountryId;
        }
        private class CountryRange : Vanrise.Entities.IDateEffectiveSettingsEditable
        {
            public DateTime BED { get; set; }
            public DateTime? EED { get; set; }
        }
		#endregion
    }
}
