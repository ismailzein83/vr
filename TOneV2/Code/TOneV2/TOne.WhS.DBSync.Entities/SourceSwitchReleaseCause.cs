using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities.EntitySynchronization;

namespace TOne.WhS.DBSync.Entities
{
    public class SourceSwitchReleaseCause : ISourceItem
    {

        public string SourceId
        {
            get;
            set;
        }
        public int SwitchID { get; set; }
        public string ReleaseCode { get; set; }
        public string Description { get; set; }
        public bool IsDelivered { get; set; }
        public string IsoCode { get; set; }
    }
}
