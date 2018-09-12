using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
	public class CustomerZoneRoutingProductHistoryLocator
	{
		#region Fields
		private CustomerZoneRoutingProductHistoryReader _reader;
		private IEnumerable<SaleEntityZoneRoutingProductSource> _orderedRoutingProductSources;
		private CustomerCountryManager _customerCountryManager;
		#endregion

		#region Constructors
		public CustomerZoneRoutingProductHistoryLocator(CustomerZoneRoutingProductHistoryReader reader)
		{
			InitializeFields(reader);
		}
		#endregion

		public IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> GetCustomerZoneRoutingProductHistory(int customerId, int sellingProductId, string zoneName, int countryId)
		{
			var routingProductHistoryBySource = new RoutingProductHistoryBySource();
			List<CustomerCountry2> customerCountries = GetAllCustomerCountries(customerId, countryId);

			if (_orderedRoutingProductSources.Contains(SaleEntityZoneRoutingProductSource.ProductDefault))
				AddProductDefaultRoutingProductHistory(routingProductHistoryBySource, sellingProductId, customerCountries);

			if (_orderedRoutingProductSources.Contains(SaleEntityZoneRoutingProductSource.ProductZone))
				AddProductZoneRoutingProductHistory(routingProductHistoryBySource, sellingProductId, zoneName, customerCountries);

			if (_orderedRoutingProductSources.Contains(SaleEntityZoneRoutingProductSource.CustomerDefault))
				AddCustomerDefaultRoutingProductHistory(routingProductHistoryBySource, customerId);

			if (_orderedRoutingProductSources.Contains(SaleEntityZoneRoutingProductSource.CustomerZone))
				AddCustomerZoneRoutingProductHistory(routingProductHistoryBySource, customerId, zoneName);

			return ZoneRoutingProductHistoryUtilities.GetZoneRoutingProductHistory(routingProductHistoryBySource, _orderedRoutingProductSources);
		}

		public SaleEntityZoneRoutingProductHistoryRecord GetCustomerZoneRoutingProductHistoryRecord(int customerId, int sellingProductId, string zoneName, int countryId, DateTime effectiveOn)
		{
			var productZoneRoutingProductHistory = GetCustomerZoneRoutingProductHistory(customerId, sellingProductId, zoneName, countryId);
			return (productZoneRoutingProductHistory != null) ? productZoneRoutingProductHistory.FindRecord(item => item.IsEffective(effectiveOn)) : null;
		}
		#region Private Methods
		private void InitializeFields(CustomerZoneRoutingProductHistoryReader reader)
		{
			_reader = reader;
			_customerCountryManager = new CustomerCountryManager();

			_orderedRoutingProductSources = new List<SaleEntityZoneRoutingProductSource>()
			{
				SaleEntityZoneRoutingProductSource.ProductDefault,
				SaleEntityZoneRoutingProductSource.ProductZone,
				SaleEntityZoneRoutingProductSource.CustomerDefault,
				SaleEntityZoneRoutingProductSource.CustomerZone
			};
		}
		private List<CustomerCountry2> GetAllCustomerCountries(int customerId, int countryId)
		{
			IEnumerable<CustomerCountry2> allCustomerCountries = _customerCountryManager.GetCustomerCountries(customerId);
			ThrowIfNullOrEmpty(allCustomerCountries, "allCustomerCountries");

			IEnumerable<CustomerCountry2> customerCountries = allCustomerCountries.FindAllRecords(x => x.CountryId == countryId);
			ThrowIfNullOrEmpty(customerCountries, "customerCountries");

			return customerCountries.OrderBy(x => x.BED).ToList();
		}
		private void ThrowIfNullOrEmpty<T>(IEnumerable<T> list, string errorMessage)
		{
			if (list == null || list.Count() == 0)
				throw new NullReferenceException(errorMessage);
		}

		private void AddProductDefaultRoutingProductHistory(RoutingProductHistoryBySource routingProductHistoryBySource, int sellingProductId, List<CustomerCountry2> customerCountries)
		{
			IEnumerable<DefaultRoutingProduct> productDefaultRoutingProducts = _reader.GetProductDefaultRoutingProducts(sellingProductId);

			if (productDefaultRoutingProducts == null || productDefaultRoutingProducts.Count() == 0)
				return;

			List<DefaultRoutingProduct> productDefaultRoutingProductsList = productDefaultRoutingProducts.ToList();
			IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> defaultRoutingProductHistory = Utilities.GetQIntersectT(customerCountries, productDefaultRoutingProductsList, ZoneRoutingProductHistoryUtilities.DefaultRoutingProductMapperAction);

			if (defaultRoutingProductHistory != null && defaultRoutingProductHistory.Count() > 0)
				routingProductHistoryBySource.AddRoutingProductHistoryRange(SaleEntityZoneRoutingProductSource.ProductDefault, defaultRoutingProductHistory);
		}
		private void AddProductZoneRoutingProductHistory(RoutingProductHistoryBySource routingProductHistoryBySource, int sellingProductId, string zoneName, List<CustomerCountry2> customerCountries)
		{
			IEnumerable<SaleZoneRoutingProduct> productZoneRoutingProducts = _reader.GetProductZoneRoutingProducts(sellingProductId, zoneName);

			if (productZoneRoutingProducts == null || productZoneRoutingProducts.Count() == 0)
				return;

			List<SaleZoneRoutingProduct> productZoneRoutingProductsList = productZoneRoutingProducts.ToList();
			IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> productZoneRoutingProductHistory = Utilities.GetQIntersectT(customerCountries, productZoneRoutingProductsList, ZoneRoutingProductHistoryUtilities.ZoneRoutingProductMapperAction);

			if (productZoneRoutingProductHistory != null && productZoneRoutingProductHistory.Count() > 0)
				routingProductHistoryBySource.AddRoutingProductHistoryRange(SaleEntityZoneRoutingProductSource.ProductZone, productZoneRoutingProductHistory);
		}
		private void AddCustomerDefaultRoutingProductHistory(RoutingProductHistoryBySource routingProductHistoryBySource, int customerId)
		{
			IEnumerable<DefaultRoutingProduct> customerDefaultRoutingProducts = _reader.GetCustomerDefaultRoutingProducts(customerId);

			if (customerDefaultRoutingProducts == null || customerDefaultRoutingProducts.Count() == 0)
				return;

			IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> customerDefaultRoutingProductHistory = customerDefaultRoutingProducts.MapRecords(ZoneRoutingProductHistoryUtilities.DefaultRoutingProductMapperFunc);
			routingProductHistoryBySource.AddRoutingProductHistoryRange(SaleEntityZoneRoutingProductSource.CustomerDefault, customerDefaultRoutingProductHistory);
		}
		private void AddCustomerZoneRoutingProductHistory(RoutingProductHistoryBySource routingProductHistoryBySource, int customerId, string zoneName)
		{
			IEnumerable<SaleZoneRoutingProduct> customerZoneRoutingProducts = _reader.GetCustomerZoneRoutingProducts(customerId, zoneName);

			if (customerZoneRoutingProducts == null || customerZoneRoutingProducts.Count() == 0)
				return;

			IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> customerZoneRoutingProductHistory = customerZoneRoutingProducts.MapRecords(ZoneRoutingProductHistoryUtilities.ZoneRoutingProductMapperFunc);
			routingProductHistoryBySource.AddRoutingProductHistoryRange(SaleEntityZoneRoutingProductSource.CustomerZone, customerZoneRoutingProductHistory);
		}
		#endregion
	}
}
