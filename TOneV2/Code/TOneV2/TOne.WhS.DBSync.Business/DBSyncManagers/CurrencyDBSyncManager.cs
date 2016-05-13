using System.Collections.Generic;
using TOne.WhS.DBSync.Data.SQL;
using Vanrise.Entities;


namespace TOne.WhS.DBSync.Business
{
    public class CurrencyDBSyncManager
    {

        bool _UseTempTables;
        public CurrencyDBSyncManager(bool useTempTables)
        {
            _UseTempTables = useTempTables;
        }

        public void ApplyCurrenciesToTemp(List<Currency> currencies)
        {
            CurrencyDBSyncDataManager dataManager = new CurrencyDBSyncDataManager(_UseTempTables);
            dataManager.ApplyCurrenciesToTemp(currencies);
        }


        public List<Currency> GetCurrencies()
        {
            CurrencyDBSyncDataManager dataManager = new CurrencyDBSyncDataManager(_UseTempTables);
            return dataManager.GetCurrencies();
        }
    }
}
