using System;
using System.Collections.Generic;

namespace Vanrise.Notification.Entities
{
    public class BaseVRNotificationQuery
    {
        public List<Guid> AlertLevelIds { get; set; }

        public string Description { get; set; }

        public List<int> StatusIds { get; set; }

        public DateTime? To { get; set; }
    }

    public class VRNotificationQuery : BaseVRNotificationQuery
    {
        public DateTime? From { get; set; }
    }

    public class VRNotificationExportQuery : BaseVRNotificationQuery
    {
        public DateTime From { get; set; }
    }
}