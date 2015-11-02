using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface ICurrencyDataManager : IDataManager
    {
        List<Currency> GetCurrencies();

        bool AreCurrenciesUpdated(ref object updateHandle);
    }
}
