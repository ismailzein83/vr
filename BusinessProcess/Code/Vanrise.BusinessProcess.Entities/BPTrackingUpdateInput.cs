using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPTrackingUpdateInput
    {
        public long GreaterThanID { get; set; }
        public int NbOfRows { get; set; }
        public int BPInstanceID { get; set; }
        public List<LogEntryType> Severities { get; set; }
    }
}