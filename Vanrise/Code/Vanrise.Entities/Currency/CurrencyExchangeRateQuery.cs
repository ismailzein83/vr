using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class CurrencyExchangeRateQuery
    {
        public List<int> Currencies { get; set; }

        public DateTime? ExchangeDate { get; set; }
    }
}
