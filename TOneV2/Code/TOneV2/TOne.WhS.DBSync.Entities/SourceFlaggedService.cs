using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities.EntitySynchronization;

namespace TOne.WhS.DBSync.Entities
{
    public class SourceFlaggedService : ISourceItem
    {
        public string SourceId
        {
            get;
            set;
        }
        public Int16 FlaggedServiceId { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ServiceColor { get; set; }

    }
}
