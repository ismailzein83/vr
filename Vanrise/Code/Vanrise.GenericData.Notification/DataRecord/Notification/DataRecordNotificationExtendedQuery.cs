using Vanrise.GenericData.Entities;
using Vanrise.Notification.Entities;

namespace Vanrise.GenericData.Notification
{
    public class DataRecordNotificationExtendedQuery : VRNotificationExtendedQuery
    {
        public RecordFilterGroup FilterGroup { get; set; }
    }
}
