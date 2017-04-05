using System;
using System.Collections.Generic;
using Vanrise.Notification.Entities;

namespace Vanrise.GenericData.Notification
{
    public class DataRecordActionTargetType : VRActionTargetType
    {
        public virtual Guid DataRecordTypeId { get; set; }

        public List<string> AvailableDataRecordFieldNames { get; set; }
    }
}
