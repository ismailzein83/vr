using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
   public class ExchangeRate
    {
       public Int64 CurrencyExchangeRateID{get;set;}
       public string CurrencyID {get;set;}

           public float Rate { get; set;}
           public DateTime ExchangeDate {get;set;}
    }
}
