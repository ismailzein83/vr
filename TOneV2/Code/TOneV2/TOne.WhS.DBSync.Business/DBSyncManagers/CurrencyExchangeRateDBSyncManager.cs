using System.Collections.Generic;
using TOne.WhS.DBSync.Data.SQL;
using Vanrise.Entities;


namespace TOne.WhS.DBSync.Business
{
    public class CurrencyExchangeRateDBSyncManager
    {
        bool _UseTempTables;
        public CurrencyExchangeRateDBSyncManager(bool useTempTables)
        {
            _UseTempTables = useTempTables;
        }

        public void ApplyCurrencyExchangeRatesToTemp(List<CurrencyExchangeRate> currencyExchangeRates)
        {
            CurrencyExchangeRateDBSyncDataManager dataManager = new CurrencyExchangeRateDBSyncDataManager(_UseTempTables);
            dataManager.ApplyCurrencyExchangeRatesToTemp(currencyExchangeRates);
        }
    }
}
