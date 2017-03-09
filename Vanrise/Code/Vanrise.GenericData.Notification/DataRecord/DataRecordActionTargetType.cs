using System;
using Vanrise.Notification.Entities;

namespace Vanrise.GenericData.Notification
{
    public class DataRecordActionTargetType : VRActionTargetType
    {
        public virtual Guid DataRecordTypeId { get; set; }
    }
}
