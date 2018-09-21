using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Common;

namespace TOne.WhS.CodePreparation.Business
{
	public class ReadRatesToAddChanges : ISaleRateReader
	{
		private SaleRatesByOwner _allSaleRatesByOwner;

		public ReadRatesToAddChanges(IEnumerable<RateToAdd> allRates)
		{
			_allSaleRatesByOwner = GetAllSaleRates(allRates);
		}

		public SaleRatesByZone GetZoneRates(SalePriceListOwnerType ownerType, int ownerId)
		{
			if (_allSaleRatesByOwner == null)
				return null;

			var saleRateByOwnerType = ownerType == SalePriceListOwnerType.Customer
				? _allSaleRatesByOwner.SaleRatesByCustomer
				: _allSaleRatesByOwner.SaleRatesByProduct;

			return saleRateByOwnerType == null ? null : saleRateByOwnerType.GetRecord(ownerId);
		}
		private SaleRatesByOwner GetAllSaleRates(IEnumerable<RateToAdd> allRates)
		{
			SaleRatesByOwner result = new SaleRatesByOwner
			{
				SaleRatesByCustomer = new VRDictionary<int, SaleRatesByZone>(),
				SaleRatesByProduct = new VRDictionary<int, SaleRatesByZone>()
			};
			SaleRatesByZone saleRateByZone;
			SaleRatePriceList saleRatePriceList;
			foreach (var rateToAdd in allRates)
			{
				if (rateToAdd.PriceListToAdd == null)
					throw new Exception(string.Format("Rate To Add on zone {0} without Pricelist assigned", rateToAdd.ZoneName));

				VRDictionary<int, SaleRatesByZone> saleRatesByOwner =
					rateToAdd.PriceListToAdd.OwnerType == SalePriceListOwnerType.SellingProduct ? result.SaleRatesByProduct : result.SaleRatesByCustomer;

				saleRateByZone = saleRatesByOwner.GetOrCreateItem(rateToAdd.PriceListToAdd.OwnerId);
				var existingRateCreationTime = rateToAdd.AddedRates.Any() ? rateToAdd.AddedRates.Last() : null;
				if (existingRateCreationTime == null)
					throw new Exception(string.Format("Opening a rate without added rates for zone {0}", rateToAdd.ZoneName));
				var existingZoneIdAtRateCreationTime = existingRateCreationTime.AddedZone;
				if (existingZoneIdAtRateCreationTime == null)
					throw new Exception(string.Format("Opening a rate without added zones for zone {0}", rateToAdd.ZoneName));
				saleRatePriceList = saleRateByZone.GetOrCreateItem(existingZoneIdAtRateCreationTime.ZoneId);
				saleRatePriceList.Rate = GetSaleRateFromRateToAdd(rateToAdd, existingRateCreationTime.RateId, existingZoneIdAtRateCreationTime.ZoneId);
			}

			return result;
		}

		private SaleRate GetSaleRateFromRateToAdd(RateToAdd rateToAdd, long rateId, long zoneId)
		{
			return new SaleRate
			{
				SaleRateId = rateId,
				ZoneId = zoneId,
				Rate = rateToAdd.Rate,
				CurrencyId = rateToAdd.CurrencyId,
				BED = rateToAdd.AddedRates.First().BED,
				RateChange = RateChangeType.New
			};
		}
	}

	public class ReadZonesRoutingProductsToAddChanges : ISaleEntityRoutingProductReader
	{
		private SaleZoneRoutingProductsByOwner _allSaleZonesRoutingProductsByOwner;
		private DefaultRoutingProductsByOwner _defaultRoutingProductsByOwner;
		ISaleEntityRoutingProductDataManager _saleEntityRoutingProductDataManager;

		public ReadZonesRoutingProductsToAddChanges(IEnumerable<ZoneRoutingProductToAdd> allZonesRoutingProducts, IEnumerable<int> customerIds, DateTime? effectiveOn, bool isEffectiveInFuture)
		{
			_saleEntityRoutingProductDataManager = BEDataManagerFactory.GetDataManager<ISaleEntityRoutingProductDataManager>();
			_allSaleZonesRoutingProductsByOwner = GetAllSaleZonesRoutingProductsByOwner(allZonesRoutingProducts);
			_defaultRoutingProductsByOwner = GetAllDefaultRoutingProductsByOwner(customerIds, effectiveOn, isEffectiveInFuture);
		}

		public DefaultRoutingProductsByOwner GetAllDefaultRoutingProductsByOwner(IEnumerable<int> customerIds, DateTime? effectiveOn, bool isEffectiveInFuture)
		{
			DefaultRoutingProductsByOwner result = new DefaultRoutingProductsByOwner();
			result.DefaultRoutingProductsByCustomer = new Dictionary<int, DefaultRoutingProduct>();
			result.DefaultRoutingProductsByProduct = new Dictionary<int, DefaultRoutingProduct>();
			List<DefaultRoutingProduct> defaultRoutingProducts = _saleEntityRoutingProductDataManager.GetDefaultRoutingProducts(customerIds, effectiveOn, isEffectiveInFuture).ToList();

			foreach (DefaultRoutingProduct defaultRoutingProduct in defaultRoutingProducts)
			{
				Dictionary<int, DefaultRoutingProduct> defaultRoutingProductsByOwner = defaultRoutingProduct.OwnerType == SalePriceListOwnerType.Customer ? result.DefaultRoutingProductsByCustomer : result.DefaultRoutingProductsByProduct;

				if (!defaultRoutingProductsByOwner.ContainsKey(defaultRoutingProduct.OwnerId))
					defaultRoutingProductsByOwner.Add(defaultRoutingProduct.OwnerId, defaultRoutingProduct);
			}
			return result;
		}

		private SaleZoneRoutingProductsByOwner GetAllSaleZonesRoutingProductsByOwner(IEnumerable<ZoneRoutingProductToAdd> allZonesRoutingProducts)
		{
			SaleZoneRoutingProductsByOwner result = new SaleZoneRoutingProductsByOwner
			{
				SaleZoneRoutingProductsByCustomer = new Dictionary<int, SaleZoneRoutingProductsByZone>(),
				SaleZoneRoutingProductsByProduct = new Dictionary<int, SaleZoneRoutingProductsByZone>(),
			};

			foreach (var zoneRoutingProductToAdd in allZonesRoutingProducts)
			{
				foreach (var addedRP in zoneRoutingProductToAdd.AddedZonesRoutingProducts)
				{
					SaleZoneRoutingProduct saleZoneRoutingProduct = new SaleZoneRoutingProduct
					{
						OwnerId = addedRP.OwnerId,
						OwnerType = addedRP.OwnerType,
						BED = addedRP.BED,
						EED = addedRP.EED,
						RoutingProductId = addedRP.RoutingProductId,
						SaleZoneId = addedRP.AddedZone.ZoneId,
						SaleEntityRoutingProductId = addedRP.SaleEntityRoutingProductId
					};

					var saleZoneRoutingProductsByOwner = (addedRP.OwnerType == SalePriceListOwnerType.Customer) ? result.SaleZoneRoutingProductsByCustomer : result.SaleZoneRoutingProductsByProduct;
					{
						SaleZoneRoutingProductsByZone saleZoneRoutingProductsByZone;
						if (!saleZoneRoutingProductsByOwner.TryGetValue(addedRP.OwnerId, out saleZoneRoutingProductsByZone))
						{
							saleZoneRoutingProductsByZone = new SaleZoneRoutingProductsByZone();
							saleZoneRoutingProductsByZone.Add(saleZoneRoutingProduct.SaleZoneId, saleZoneRoutingProduct);
							saleZoneRoutingProductsByOwner.Add(addedRP.OwnerId, saleZoneRoutingProductsByZone);
						}
						else if (!saleZoneRoutingProductsByZone.ContainsKey(saleZoneRoutingProduct.SaleZoneId))
						{
							saleZoneRoutingProductsByZone.Add(saleZoneRoutingProduct.SaleZoneId, saleZoneRoutingProduct);
						}
					}
				}
			}

			return result;
		}

		public DefaultRoutingProduct GetDefaultRoutingProduct(SalePriceListOwnerType ownerType, int ownerId, long? zoneId)
		{
			DefaultRoutingProduct defaultRoutingProduct = null;
			if (zoneId.HasValue)
			{
				if (ownerType == SalePriceListOwnerType.Customer && _defaultRoutingProductsByOwner != null && _defaultRoutingProductsByOwner.DefaultRoutingProductsByCustomer != null)
					_defaultRoutingProductsByOwner.DefaultRoutingProductsByCustomer.TryGetValue(ownerId, out defaultRoutingProduct);
				if (ownerType == SalePriceListOwnerType.SellingProduct && _defaultRoutingProductsByOwner != null && _defaultRoutingProductsByOwner.DefaultRoutingProductsByProduct != null)
					_defaultRoutingProductsByOwner.DefaultRoutingProductsByProduct.TryGetValue(ownerId, out defaultRoutingProduct);
			}
			return defaultRoutingProduct;
		}

		public SaleZoneRoutingProductsByZone GetRoutingProductsOnZones(SalePriceListOwnerType ownerType, int ownerId)
		{
			if (_allSaleZonesRoutingProductsByOwner != null)
			{
				var saleZoneRoutingProductsByOwner = (ownerType == SalePriceListOwnerType.Customer) ? _allSaleZonesRoutingProductsByOwner.SaleZoneRoutingProductsByCustomer : _allSaleZonesRoutingProductsByOwner.SaleZoneRoutingProductsByProduct;
				SaleZoneRoutingProductsByZone saleZoneRoutingProductByZone;
				saleZoneRoutingProductsByOwner.TryGetValue(ownerId, out saleZoneRoutingProductByZone);
				return saleZoneRoutingProductByZone;
			}
			return null;
		}

	}
}
