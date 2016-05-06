using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.DBSync.Entities
{
    public class SourceCurrencyExchangeRate : Vanrise.Entities.EntitySynchronization.ISourceItem
    {

        public string SourceId
        {
            get;
            set;
        }

        public decimal? Rate { get; set; }

        public DateTime? ExchangeDate { get; set; }

        public string CurrencyId { get; set; }

    }
}
