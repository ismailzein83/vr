using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;

namespace Vanrise.AccountBalance.Entities
{
    public class LiveBalance : IVREntityBalanceInfo
    {
        public long AccountId { get; set; }
        public Guid AccountTypeId { get; set; }
        public decimal InitialBalance { get; set; }
        public Decimal UsageBalance { get; set; }
        public int? AlertRuleID { get; set; }
        public int? ThresholdIndex { get; set; }
        public Decimal CurrentBalance { get; set; }
        public int CurrencyId { get; set; }
        public decimal? LastExecutedThreshold { get; set; }
        public decimal? NextThreshold { get; set; }

        public string EntityId
        {
            get { return this.AccountId.ToString(); }
        }

        public decimal? NextAlertThreshold
        {
            get { return this.NextThreshold; }
        }

        public decimal? LastExecutedAlertThreshold
        {
            get { return this.LastExecutedThreshold; }
        }

        public int? AlertRuleId
        {
            get { return this.AlertRuleID; }
        }


        public int? ThresholdActionIndex
        {
            get { return this.ThresholdIndex; }
        }
    }
}
