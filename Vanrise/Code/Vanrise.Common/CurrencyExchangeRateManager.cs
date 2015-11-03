using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common
{
    public class CurrencyExchangeRateManager
    {
        public CurrencyExchangeRate GetEffectiveExchangeRate(int currencyId, DateTime effectiveOn)
        {
            throw new NotImplementedException();
        }

        public Decimal ConvertValueToCurrency(decimal value, int currencyId, DateTime effectiveOn)
        {
            var exchangeRate = GetEffectiveExchangeRate(currencyId, effectiveOn);
            if (exchangeRate != null)
                return value * exchangeRate.Rate;
            else
                return value;

        }
    }
}
