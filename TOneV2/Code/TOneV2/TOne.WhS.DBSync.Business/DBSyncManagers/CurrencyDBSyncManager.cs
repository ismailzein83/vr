using System.Collections.Generic;
using TOne.WhS.DBSync.Data.SQL;
using Vanrise.Entities;


namespace TOne.WhS.DBSync.Business
{
    public class CurrencyDBSyncManager
    {

        public void ApplyCurrenciesToTemp(List<Currency> currencies)
        {
            CurrencyDBSyncDataManager dataManager = new CurrencyDBSyncDataManager();
            dataManager.ApplyCurrenciesToTemp(currencies);
        }


        public Currency GetCurrencyBySourceId(string sourceId)
        {
            CurrencyDBSyncDataManager dataManager = new CurrencyDBSyncDataManager();
            return dataManager.GetCurrencyBySourceId(sourceId);
        }
    }
}
