using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;
using TOne.Caching;
using TOne.Entities;

namespace TOne.CDR.Business
{
    public class BasePricing
    {
         TOneCacheManager _cacheManager;
        protected List<Currency> Currencies;
         public BasePricing(TOneCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
             CurrencyManager currencyManager = new CurrencyManager();
             this.Currencies = currencyManager.GetCurrencies();
        }


        public List<Rate> GetRates(String customerID, int zoneID, DateTime whenEffective)
        {
            return GetEffectiveEntities<Rate>(customerID, zoneID, whenEffective);
        }
        public List<ToDConsideration> GetToDConsiderations(String customerID, int zoneID, DateTime whenEffective)
        {
            return GetEffectiveEntities<ToDConsideration>(customerID, zoneID, whenEffective);
        }
        public List<Commission> GetCommissions(String customerID, int zoneID, DateTime whenEffective)
        {
            return GetEffectiveEntities<Commission>(customerID, zoneID, whenEffective);
        }

        public List<Tariff> GetTariffs(String customerID, int zoneID, DateTime whenEffective)
        {
            return GetEffectiveEntities<Tariff>(customerID, zoneID, whenEffective);
        }
        public List<T> GetEffectiveEntities<T>(String customerID, int zoneID, DateTime whenEffective) where T : IZoneSupplied
        {
            return _cacheManager.GetOrCreateObject(String.Format("GetEffectiveEntities_{0}_{1}_{2:ddMMMyy}", typeof(T).Name, customerID, whenEffective.Date),
                CacheObjectType.Pricing,
                () =>
                {

                    DateSensitiveEntityCache<T> rates = null;
                    rates = new DateSensitiveEntityCache<T>(customerID, 0, whenEffective, true);
                    return rates;
                }).GetEffectiveEntities(customerID, zoneID, whenEffective);
        }

        public  decimal GetRate(decimal rate, Currency originalCurrency, Currency otherCurrency, DateTime date)
        {
            decimal result = rate;

            if (originalCurrency.CurrencyID.Equals(otherCurrency.CurrencyID)) return result;
            RateManager rateManager=new RateManager();
            List<ExchangeRate> exchanges=rateManager.GetExchangeRates(date);
            if (exchanges == null) return result;

            decimal? ExchangeInOriginalCurrency = originalCurrency.CurrencyID.Equals(Currencies.Find(c => c.IsMainCurrency == "Y").CurrencyID) ? 1m : (
                                                exchanges.Any(exch => exch.CurrencyID.Equals(originalCurrency.CurrencyID))
                                                ? (decimal?)exchanges.OrderByDescending(e => e.ExchangeDate).First(exch => exch.CurrencyID.Equals(originalCurrency.CurrencyID)).Rate
                                                : null);

            decimal? ExchangeInOtherCurrency = otherCurrency.CurrencyID.Equals(Currencies.Find(c => c.IsMainCurrency == "Y").CurrencyID) ? 1m : (
                                                exchanges.Any(exch => exch.CurrencyID.Equals(otherCurrency.CurrencyID))
                                                ? (decimal?)exchanges.OrderByDescending(e => e.ExchangeDate).First(exch => exch.CurrencyID.Equals(otherCurrency.CurrencyID)).Rate
                                                : null);

            if (ExchangeInOriginalCurrency == null || ExchangeInOtherCurrency == null) return result;

            result = result * (decimal)ExchangeInOtherCurrency / (decimal)ExchangeInOriginalCurrency;

            return result;
        }
      
        
    }
}
