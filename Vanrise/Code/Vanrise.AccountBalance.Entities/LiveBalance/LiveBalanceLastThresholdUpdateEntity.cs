using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;

namespace Vanrise.AccountBalance.Entities
{
    public class LiveBalanceLastThresholdUpdateEntity
    {
        public Guid AccountTypeId { get; set; }
        public String AccountId { get; set; }
        public decimal? LastExecutedActionThreshold { get; set; }
        public VRBalanceActiveAlertInfo ActiveAlertsInfo { get; set; }
    }
}
