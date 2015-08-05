
using System.Collections.Generic;
using Vanrise.Common;

namespace Vanrise.BusinessProcess.Entities
{
    public class TrackingQuery
    {
        public long ProcessInstanceId { get; set; }
        public long FromTrackingId { get; set; }

        public string Message { get; set; }

        public List<LogEntryType> Severities { get; set; }
    }
}
