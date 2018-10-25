using System;

namespace Vanrise.Notification.BP.Arguments
{
    public class ExecuteNotificationProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument, INotificationProcessArgument
    {
        public override string GetTitle()
        {
            return this.ProcessTitle != null ? this.ProcessTitle : "Execute Notification Process";
        }

        public string ProcessTitle { get; set; }

        public long NotificationId { get; set; }

        public string EventKey { get; set; }

        public Guid NotificationTypeId { get; set; }

        public long? AlertRuleId { get; set; }
    }
}
