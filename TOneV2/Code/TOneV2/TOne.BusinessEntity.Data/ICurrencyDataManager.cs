using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Data
{
    public interface ICurrencyDataManager : IDataManager
    {
        List<Currency> GetCurrencies();

        Dictionary<string, Currency> GetCurrenciesDictionary();

        List<Currency> GetVisibleCurrencies();

        Currency GetCurrencyByCarrierId(string carrierId);
    }
}
