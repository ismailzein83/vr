using System;
using Vanrise.Entities.EntitySynchronization;

namespace TOne.WhS.DBSync.Entities
{
    public class SourceCode : ISourceItem
    {

        public string SourceId
        {
            get;
            set;
        }

        public string Code { get; set; }
        public int ZoneId { get; set; }
        public DateTime BeginEffectiveDate { get; set; }
        public DateTime? EndEffectiveDate { get; set; }
        public string CodeGroup { get; set; }
    }
}
