using System;
using System.Collections.Generic;

namespace Vanrise.Notification.Entities
{
    public class VRNotificationQuery
    {
        public string Description { get; set; }

        public List<int> StatusIds { get; set; }

        public List<Guid> AlertLevelIds { get; set; }
    }
}