using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.Entities
{
    public enum BPInstanceStatus
    {
        [BPInstanceStatus(TrackingSeverity = LogEntryType.Information, IsClosed = false)]
        New = 0,
        [BPInstanceStatus(TrackingSeverity = LogEntryType.Warning, IsClosed = false)]
        Postponed = 5,
        [BPInstanceStatus(TrackingSeverity = LogEntryType.Information, IsClosed = false)]
        Running = 10,
        [BPInstanceStatus(TrackingSeverity = LogEntryType.Information, IsClosed = false)]
        Waiting = 20,
        [BPInstanceStatus(TrackingSeverity = LogEntryType.Warning, IsClosed = false)]
        Cancelling = 30,
        [BPInstanceStatus(TrackingSeverity = LogEntryType.Information, IsClosed = true)]
        Completed = 50,
        [BPInstanceStatus(TrackingSeverity = LogEntryType.Error, IsClosed = true)]
        Aborted = 60,
        [BPInstanceStatus(TrackingSeverity = LogEntryType.Error, IsClosed = true)]
        Suspended = 70,
        [BPInstanceStatus(TrackingSeverity = LogEntryType.Error, IsClosed = true)]
        Terminated = 80,
        [BPInstanceStatus(TrackingSeverity = LogEntryType.Error, IsClosed = true)]
        Cancelled = 90
    }

    public enum BPInstanceAssignmentStatus
    {
        Free = 0,
        AssignedForExecution = 1,
        Executing = 2
    }

    public class BPInstanceStatusAttribute : Attribute
    {
        static Dictionary<BPInstanceStatus, BPInstanceStatusAttribute> _cachedAttributes;
        static BPInstanceStatusAttribute()
        {
            _cachedAttributes = new Dictionary<BPInstanceStatus, BPInstanceStatusAttribute>();
            foreach (var member in typeof(BPInstanceStatus).GetFields())
            {
                BPInstanceStatusAttribute mbrAttribute = member.GetCustomAttributes(typeof(BPInstanceStatusAttribute), true).FirstOrDefault() as BPInstanceStatusAttribute;
                if (mbrAttribute != null)
                    _cachedAttributes.Add((BPInstanceStatus)Enum.Parse(typeof(BPInstanceStatus), member.Name), mbrAttribute);
            }
        }

        public LogEntryType TrackingSeverity { get; set; }

        public bool IsClosed { get; set; }

        public static BPInstanceStatusAttribute GetAttribute(BPInstanceStatus status)
        {
            return _cachedAttributes[status];
        }

        public static List<BPInstanceStatus> GetNonClosedStatuses()
        {
            List<BPInstanceStatus> rslt = new List<BPInstanceStatus>();
            foreach (var statusEnum in Enum.GetValues(typeof(BPInstanceStatus)))
            {
                BPInstanceStatus status = (BPInstanceStatus)statusEnum;
                if (!GetAttribute(status).IsClosed)
                    rslt.Add(status);
            }
            return rslt;
        }
    }
}
