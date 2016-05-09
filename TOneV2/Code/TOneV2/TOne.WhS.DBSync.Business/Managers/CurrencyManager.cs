using System.Collections.Generic;
using TOne.WhS.DBSync.Data.SQL;
using Vanrise.Entities;


namespace TOne.WhS.DBSync.Business
{
    public class CurrencyManager
    {

        public void AddCurrenciesFromSource(List<Currency> currencies)
        {
            CurrencyDataManager dataManager = new CurrencyDataManager("Currency");
            dataManager.ApplyCurrenciesToDB(currencies);
        }
    }
}
