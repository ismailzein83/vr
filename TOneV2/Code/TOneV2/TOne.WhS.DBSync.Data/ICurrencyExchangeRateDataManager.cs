using System.Collections.Generic;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Data
{
    public interface ICurrencyExchangeRateDataManager : IDataManager
    {
        void MigrateCurrencyExchangeRatesToDB(List<CurrencyExchangeRate> currencyExchangeRates);
    }
}
