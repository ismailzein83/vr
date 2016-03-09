using System.Collections.Generic;
using Vanrise.Common;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPTrackingBeforeIdInput
    {
        public long LessThanID { get; set; }
        public int NbOfRows { get; set; }
        public int BPInstanceID { get; set; }
        public List<LogEntryType> Severities { get; set; }
    }
}
