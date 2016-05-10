using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities.EntitySynchronization;

namespace Vanrise.Entities
{
    public class CurrencyExchangeRate : Vanrise.Entities.EntitySynchronization.IItem
    {

        public long CurrencyExchangeRateId { get; set; }
        public int CurrencyId { get; set; }

        public Decimal Rate { get; set; }

        public DateTime ExchangeDate { get; set; }

        public string SourceID { get; set; }

        long IItem.ItemId
        {
            get
            {
                return CurrencyExchangeRateId;
            }
            set
            {
                this.CurrencyExchangeRateId = (int)value;
            }
        }
    }
}
