using System;

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
