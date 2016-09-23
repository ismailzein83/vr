using System;
using Vanrise.Entities.EntitySynchronization;

namespace TOne.WhS.DBSync.Entities
{
    public class SourceCustomTimeZoneInfo : ISourceItem
    {

        public string SourceId
        {
            get;
            set;
        }

        public int BaseUtcOffset { get; set; }
        public string DisplayName { get; set; }
    }
}
