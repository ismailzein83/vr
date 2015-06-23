using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vanrise.BusinessProcess.Entities
{
    public enum BPInstanceStatus
    {
        [BPInstanceStatus(TrackingSeverity = BPTrackingSeverity.Information, IsClosed=false)]
        New = 0,
        [BPInstanceStatus(TrackingSeverity = BPTrackingSeverity.Information, IsClosed = false)]
        Running = 10,
        [BPInstanceStatus(TrackingSeverity = BPTrackingSeverity.Error, IsClosed = false)]
        ProcessFailed = 20,
        [BPInstanceStatus(TrackingSeverity = BPTrackingSeverity.Information, IsClosed = true)]
        Completed = 50,
        [BPInstanceStatus(TrackingSeverity = BPTrackingSeverity.Error, IsClosed = true)]
        Aborted = 60,
        [BPInstanceStatus(TrackingSeverity = BPTrackingSeverity.Error, IsClosed = true)]
        Suspended = 70,
        [BPInstanceStatus(TrackingSeverity = BPTrackingSeverity.Error, IsClosed = true)]
        Terminated = 80
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

        public BPTrackingSeverity TrackingSeverity { get; set; }

        public bool IsClosed { get; set; }

        public static BPInstanceStatusAttribute GetAttribute(BPInstanceStatus status)
        {
            return _cachedAttributes[status];
        }

    }
}
