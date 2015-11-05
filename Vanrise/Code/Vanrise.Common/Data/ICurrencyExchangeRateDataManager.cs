using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface ICurrencyExchangeRateDataManager : IDataManager
    {
        List<CurrencyExchangeRate> GetCurrenciesExchangeRate();
        bool Insert(CurrencyExchangeRate currencyExchangeRate, out int insertedId);
        bool Update(CurrencyExchangeRate currencyExchangeRate);
        bool AreCurrenciesExchangeRateUpdated(ref object updateHandle);
    }
}
