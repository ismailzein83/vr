using System;
using Vanrise.Entities.EntitySynchronization;

namespace TOne.WhS.DBSync.Entities
{
    public class SourceZone : ISourceItem
    {

        public string SourceId
        {
            get;
            set;
        }

        public string Name { get; set; }
        public string CodeGroup { get; set; }
        public string SupplierId { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
    }
}
