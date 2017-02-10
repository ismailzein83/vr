using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;

namespace Vanrise.AccountBalance.Entities
{
    public class LiveBalance
    {
        public String AccountId { get; set; }
        public Guid AccountTypeId { get; set; }
        public decimal InitialBalance { get; set; }
        public int? AlertRuleID { get; set; }
        public Decimal CurrentBalance { get; set; }
        public int CurrencyId { get; set; }
        public decimal? LastExecutedThreshold { get; set; }
        public decimal? NextThreshold { get; set; }
        public VRBalanceActiveAlertInfo LiveBalanceActiveAlertsInfo { get; set; }
    }
}
