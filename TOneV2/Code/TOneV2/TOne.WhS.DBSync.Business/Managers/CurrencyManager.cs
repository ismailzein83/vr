using System.Collections.Generic;
using TOne.WhS.DBSync.Data;
using Vanrise.Entities;


namespace TOne.WhS.DBSync.Business
{
    public class CurrencyManager
    {

        public void AddCurrenciesFromSource(List<Currency> currencies)
        {
            ICurrencyDataManager dataManager = BEDataManagerFactory.GetDataManager<ICurrencyDataManager>();
            dataManager.MigrateCurrenciesToDB(currencies);
        }
    }
}
