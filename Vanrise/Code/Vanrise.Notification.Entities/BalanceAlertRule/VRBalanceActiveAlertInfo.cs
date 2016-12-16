using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vanrise.Notification.Entities
{
    public class VRBalanceActiveAlertInfo
    {
        public VRBalanceActiveAlertInfo()
        {
            ActiveAlertsThersholds = new List<VRBalanceActiveAlertThreshold>();
        }
        public List<VRBalanceActiveAlertThreshold> ActiveAlertsThersholds { get; set; }
    }
}
