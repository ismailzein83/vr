using System;
using Vanrise.Entities.EntitySynchronization;

namespace TOne.WhS.DBSync.Entities
{
    public class SourceCurrencyExchangeRate : ISourceItem
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
