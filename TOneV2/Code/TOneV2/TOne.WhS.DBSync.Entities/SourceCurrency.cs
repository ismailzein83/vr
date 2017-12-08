
using System;
using Vanrise.Entities.EntitySynchronization;
namespace TOne.WhS.DBSync.Entities
{
    public class SourceCurrency : ISourceItem
    {

        public string SourceId
        {
            get;
            set;
        }

        public string Name { get; set; }

        public bool IsMainCurrency { get; set; }
        public string Symbol { get; set; }
        public decimal LastRate { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
