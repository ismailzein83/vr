using System.Collections.Generic;
using TOne.WhS.DBSync.Data.SQL;
using Vanrise.Entities;


namespace TOne.WhS.DBSync.Business
{
    public class CurrencyExchangeRateDBSyncManager
    {

        public void ApplyCurrencyExchangeRatesToTemp(List<CurrencyExchangeRate> currencyExchangeRates)
        {
            CurrencyExchangeRateDBSyncDataManager dataManager = new CurrencyExchangeRateDBSyncDataManager("Common.CurrencyExchangeRate_Temp");
            dataManager.ApplyCurrencyExchangeRatesToTemp(currencyExchangeRates);
        }
    }
}
