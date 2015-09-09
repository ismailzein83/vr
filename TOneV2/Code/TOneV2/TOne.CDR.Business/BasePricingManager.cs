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
    public class BasePricingManager
    {
         TOneCacheManager _cacheManager;
        protected List<Currency> Currencies;
         public BasePricingManager(TOneCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
             CurrencyManager currencyManager = new CurrencyManager();
             this.Currencies = currencyManager.GetCurrencies();
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
