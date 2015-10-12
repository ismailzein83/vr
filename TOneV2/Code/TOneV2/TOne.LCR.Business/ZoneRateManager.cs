using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;
using TOne.LCR.Data;
using TOne.LCR.Entities;

namespace TOne.LCR.Business
{
    public class ZoneRateManager
    {
        PriceListManager _priceListManager;
        BusinessEntityInfoManager _businessEntityInfoManager;
        CodeManager _codeManager;
        SaleZoneMarketPriceManager _saleZoneMarketPriceManager;
        CurrencyManager _currencyManager;
        ZoneManager _zoneManager;
        FlaggedServiceManager _flaggedServiceManager;
        readonly IZoneRateDataManager _dataManager;

        public ZoneRateManager()
        {
            _dataManager = LCRDataManagerFactory.GetDataManager<IZoneRateDataManager>();
            _priceListManager = new PriceListManager();
            _businessEntityInfoManager = new BusinessEntityInfoManager();
            _codeManager = new CodeManager();
            _currencyManager = new CurrencyManager();
            _zoneManager = new ZoneManager();
            _saleZoneMarketPriceManager = new SaleZoneMarketPriceManager();
            _flaggedServiceManager = new FlaggedServiceManager();
        }

        public IEnumerable<SaleRate> GetCustomerSaleZones(string customerId, string zoneName)
        {
            List<SaleRate> saleRates = new List<SaleRate>();
            CustomerSaleZones customerSaleZones = _dataManager.GetCustomerSaleZones(customerId, zoneName);
            SaleZoneMarketPrices saleMarketPrices = _saleZoneMarketPriceManager.GetAllSaleZoneMarketPrices();

            Dictionary<int, PriceList> salePriceLists = new Dictionary<int, PriceList>();
            Dictionary<string, Currency> currencies = _currencyManager.GetCurrenciesDictionary();
            Currency mainCurrency = currencies.Values.Where(c => c.IsMainCurrency.Equals("Y")).First();

            foreach (var customerSaleZone in customerSaleZones)
            {
                PriceList customerPriceList;
                if (!salePriceLists.TryGetValue(customerSaleZone.Value.PriceListId, out customerPriceList))
                {
                    customerPriceList = _priceListManager.GetPriceListById(customerSaleZone.Value.PriceListId);
                    salePriceLists.Add(customerSaleZone.Value.PriceListId, customerPriceList);
                }
                Currency customerCurrency = customerPriceList != null ? currencies[customerPriceList.CurrencyId] : mainCurrency;
                Zone saleZone = _zoneManager.GetZone(customerSaleZone.Key);
                SaleRate saleRate = new SaleRate()
                {
                    Currency = customerCurrency,
                    EffectiveCodes = _codeManager.GetCodes(customerSaleZone.Value.ZoneId, DateTime.Now).Select(c => c.Value).ToList(),
                    PriceListId = customerSaleZone.Value.PriceListId,
                    Rate = customerSaleZone.Value.Rate,
                    OurServicesFlag = _flaggedServiceManager.GetServiceFlag(customerSaleZone.Value.ServiceFlag),
                    ZoneId = customerSaleZone.Value.ZoneId,
                    ZoneName = customerSaleZone.Value.ZoneName,
                    CodeGroup = saleZone != null ? saleZone.CodeGroupId : "999999"
                };

                List<SupplierLCR> marketPriceSupplierLcr = GetMarketPriceSuppliersLCR(customerSaleZone.Value, saleMarketPrices, customerCurrency, mainCurrency, currencies);
                ApplySupplierZoneRates(saleRate, marketPriceSupplierLcr, customerCurrency, mainCurrency, currencies);
                saleRates.Add(saleRate);
            }

            return saleRates;
        }

        private void ApplySupplierZoneRates(SaleRate saleRate, List<SupplierLCR> marketPriceSupplierLcr, Currency currency, Currency mainCurrency, Dictionary<string, Currency> currencies)
        {
            bool IsCodeGroup = (saleRate.EffectiveCodes.Count == 1 && saleRate.EffectiveCodes[0].Equals(saleRate.CodeGroup));
            List<SupplierZoneRate> supplierZoneRates = new List<SupplierZoneRate>();
            foreach (var supplierLCR in marketPriceSupplierLcr.GroupBy(l => l.SupplierId))
            {

                if (IsCodeGroup == true)
                {
                    supplierZoneRates.AddRange(GetSupplierZoneRate(supplierLCR, mainCurrency, currencies).Where(l => l.IsCodeGroup));
                    continue;
                }

                bool isLandlineExistsInSupplyRates = false;
                isLandlineExistsInSupplyRates = supplierLCR.Any(sr => sr.IsCodeGroup);

                bool OnlyLandlineMatch = isLandlineExistsInSupplyRates ? supplierLCR.Where(sr => sr.IsCodeGroup).Count() == 1 && supplierLCR.Count() == 1 : false;
                if (OnlyLandlineMatch)
                {
                    supplierZoneRates.AddRange(GetSupplierZoneRate(supplierLCR, mainCurrency, currencies));
                    continue;
                }
                if (!isLandlineExistsInSupplyRates)
                {
                    supplierZoneRates.AddRange(GetSupplierZoneRate(supplierLCR, mainCurrency, currencies));
                    continue;
                }
            }
            supplierZoneRates = supplierZoneRates.OrderBy(r => r.Rate * decimal.Parse(_currencyManager.GetExchangeFactor(r.Currency, currency).ToString())).Take(3).ToList();
            supplierZoneRates.ForEach(r => r.Rate = r.Rate * decimal.Parse(_currencyManager.GetExchangeFactor(r.Currency, currency).ToString()));
            saleRate.SupplierZoneRates = supplierZoneRates;
        }

        private IEnumerable<SupplierZoneRate> GetSupplierZoneRate(IGrouping<string, SupplierLCR> suppliersLCR, Currency mainCurrency, Dictionary<string, Currency> currencies)
        {
            Dictionary<int, PriceList> priceLists = new Dictionary<int, PriceList>();
            List<SupplierZoneRate> supplierZoneRates = new List<SupplierZoneRate>();
            foreach (var supplierLCR in suppliersLCR)
            {
                int priceListId = supplierLCR.PriceListId;
                PriceList supplierPriceList;
                if (!priceLists.TryGetValue(priceListId, out supplierPriceList))
                {
                    supplierPriceList = _priceListManager.GetPriceListById(priceListId);
                    priceLists.Add(priceListId, supplierPriceList);
                }

                SupplierZoneRate supplierZoneRate = new SupplierZoneRate()
                {
                    IsCodeGroup = supplierLCR.IsCodeGroup,
                    PriceListId = supplierLCR.PriceListId,
                    Rate = supplierLCR.Rate,
                    SupplierServicesFlag = _flaggedServiceManager.GetServiceFlag(supplierLCR.ServiceFlag),
                    SupplierId = supplierLCR.SupplierId,
                    SupplierName = _businessEntityInfoManager.GetCarrirAccountName(supplierLCR.SupplierId),
                    ZoneId = supplierLCR.ZoneId,
                    ZoneName = supplierLCR.ZoneName,
                    Currency = supplierPriceList != null ? currencies[supplierPriceList.CurrencyId] : mainCurrency
                };
                supplierZoneRates.Add(supplierZoneRate);
            }
            return supplierZoneRates;
        }

        private List<SupplierLCR> GetMarketPriceSuppliersLCR(CustomerSaleZone customerSaleZone, SaleZoneMarketPrices saleMarketPrices, Currency customerCurrency, Currency mainCurrency, Dictionary<string, Currency> currencies)
        {
            List<SupplierLCR> suppliersLCR = new List<SupplierLCR>();
            SaleZoneMarketPrice saleMarketPrice;
            Dictionary<int, PriceList> priceLists = new Dictionary<int, PriceList>();
            if (saleMarketPrices.TryGetValue(customerSaleZone.ZoneId + "-" + customerSaleZone.ServiceFlag, out saleMarketPrice))
            {
                foreach (var supplierLCR in customerSaleZone.SuppliersLcr)
                {

                    PriceList supplierPriceList;
                    if (!priceLists.TryGetValue(supplierLCR.PriceListId, out supplierPriceList))
                    {
                        supplierPriceList = _priceListManager.GetPriceListById(supplierLCR.PriceListId);
                        priceLists.Add(supplierLCR.PriceListId, supplierPriceList);
                    }
                    Currency supplierCurrency = supplierPriceList != null ? currencies[supplierPriceList.CurrencyId] : mainCurrency;
                    double currencyExchangeFactor = _currencyManager.GetExchangeFactor(supplierCurrency, customerCurrency);
                    if (!(Math.Round(saleMarketPrice.FromRate * (decimal)currencyExchangeFactor, 4) > Math.Round(customerSaleZone.Rate * (decimal)currencyExchangeFactor, 4)
            || Math.Round(saleMarketPrice.ToRate * (decimal)currencyExchangeFactor, 4) < Math.Round(customerSaleZone.Rate * (decimal)currencyExchangeFactor, 4)))
                    {
                        suppliersLCR.Add(supplierLCR);
                    }
                }
            }
            else
                suppliersLCR.AddRange(customerSaleZone.SuppliersLcr);
            return suppliersLCR;
        }
    }
}
