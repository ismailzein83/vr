using System.Collections.Generic;
using TOne.WhS.DBSync.Data.SQL;
using Vanrise.Entities;


namespace TOne.WhS.DBSync.Business
{
    public class CurrencyExchangeRateManager
    {

        public void AddCurrencyExchangeRatesFromSource(List<CurrencyExchangeRate> currencyExchangeRates)
        {
            CurrencyExchangeRateDataManager dataManager = new CurrencyExchangeRateDataManager("CurrencyExchangeRate_Temp");
            dataManager.ApplyCurrencyExchangeRatesToDB(currencyExchangeRates);
        }
    }
}
