using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;
using TOne.Entities;
using Vanrise.Entities;

namespace TOne.BusinessEntity.Business
{
    public class CurrencyManager
    {
        ICurrencyDataManager _dataManager;

        public CurrencyManager()
        {
            _dataManager = BEDataManagerFactory.GetDataManager<ICurrencyDataManager>();
        }

        public List<Currency> GetCurrencies()
        {
            return _dataManager.GetCurrencies();
        }

        public List<Currency> GetVisibleCurrencies()
        {
            return _dataManager.GetVisibleCurrencies();
        }

        public Dictionary<string, Currency> GetCurrenciesDictionary()
        {
            return _dataManager.GetCurrenciesDictionary();
        }

        public Currency GetCurrencyByCarrierId(string carrierId)
        {
            return _dataManager.GetCurrencyByCarrierId(carrierId);
        }

        public IEnumerable<CurrencyFactor> GetCurrenciesFactor(string customerId)
        {
            List<Currency> visibleCurrencies = GetVisibleCurrencies();
            Currency customerCurrency = GetCurrencyByCarrierId(customerId);
            List<CurrencyFactor> currencyFactors = new List<CurrencyFactor>();

            foreach (Currency currency in visibleCurrencies)
            {
                CurrencyFactor currencyFactor = new CurrencyFactor();
                if (currency.CurrencyID.Equals(customerCurrency.CurrencyID))
                    currencyFactor.IsMain = true;
                currencyFactor.CurrencyId = currency.CurrencyID;
                currencyFactor.Factor = GetExchangeFactor(customerCurrency, currency);
                currencyFactor.Name = currency.Name;
                currencyFactors.Add(currencyFactor);
            }

            return currencyFactors;
        }

        public double GetExchangeFactor(Currency fromCurrency, Currency toCurrency)
        {
            return toCurrency.LastRate / fromCurrency.LastRate;
        }
    }
}
