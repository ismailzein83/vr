using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vanrise.BusinessProcess
{
    public enum BPInstanceStatus
    {
        [BPInstanceStatus(TrackingSeverity = BPTrackingSeverity.Information)]
        New = 0,
        [BPInstanceStatus(TrackingSeverity = BPTrackingSeverity.Information)]
        Running = 10,
        [BPInstanceStatus(TrackingSeverity = BPTrackingSeverity.Error)]
        ProcessFailed = 20,
        [BPInstanceStatus(TrackingSeverity = BPTrackingSeverity.Information)]
        Completed = 50,
        [BPInstanceStatus(TrackingSeverity = BPTrackingSeverity.Error)]
        Aborted = 60,
        [BPInstanceStatus(TrackingSeverity = BPTrackingSeverity.Error)]
        Suspended = 70,
        [BPInstanceStatus(TrackingSeverity = BPTrackingSeverity.Error)]
        Terminated = 80
    }

    internal class BPInstanceStatusAttribute : Attribute
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

        public BPTrackingSeverity TrackingSeverity { get; set; }

        public static BPInstanceStatusAttribute GetAttribute(BPInstanceStatus status)
        {
            return _cachedAttributes[status];
        }
    }
}
