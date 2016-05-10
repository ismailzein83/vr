using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
namespace Vanrise.Common.Business
{
    public class ExchangeRateInfo
    {
        public string Symbol { get; set; }

        public int CurrencyId { get; set; }
        public CurrencyExchangeRate ExchangeRate {get ; set;}
    }
}
