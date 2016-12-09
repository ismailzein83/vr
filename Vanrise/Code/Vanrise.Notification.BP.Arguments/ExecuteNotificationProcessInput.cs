using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;

namespace Vanrise.Notification.BP.Arguments
{
    public class ExecuteNotificationProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public override string GetTitle()
        {
            return "Execute Notification Process";
        }

        public Guid NotificationId { get; set; }
    }
}
