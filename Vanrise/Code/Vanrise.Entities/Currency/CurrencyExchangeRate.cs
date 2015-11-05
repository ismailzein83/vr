using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class CurrencyExchangeRate
    {

        public long CurrencyExchangeRateId { get; set; }
        public int CurrencyId { get; set; }

        public Decimal Rate { get; set; }

        public DateTime ExchangeDate { get; set; }
    }
}
