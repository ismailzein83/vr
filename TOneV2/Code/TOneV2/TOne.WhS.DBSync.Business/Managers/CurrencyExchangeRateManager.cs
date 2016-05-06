using System.Collections.Generic;
using TOne.WhS.DBSync.Data;
using Vanrise.Entities;


namespace TOne.WhS.DBSync.Business
{
    public class CurrencyExchangeRateManager
    {

        public void AddCurrencyExchangeRatesFromSource(List<CurrencyExchangeRate> currencyExchangeRates)
        {
            ICurrencyExchangeRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ICurrencyExchangeRateDataManager>();
            dataManager.MigrateCurrencyExchangeRatesToDB(currencyExchangeRates);
        }
    }
}
