using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class CurrencyExchangeRateDetail
    {

        public CurrencyExchangeRate Entity { get; set; }
        public string CurrencySymbol { get; set; }
        public string CurrencyName { get; set; }
        public bool? IsMain { get; set; }
        public string RateDescription { get; set; }
    }
}
