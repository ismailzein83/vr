using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class LiveBalance : Vanrise.Notification.Entities.IVREntityBalanceInfo
    {
        public long AccountId { get; set; }
        public decimal InitialBalance { get; set; }
        public Decimal UsageBalance { get; set; }
        public int? AlertRuleID { get; set; }
        public Decimal CurrentBalance { get; set; }
        public int CurrencyId { get; set; }

        public string EntityId
        {
            get { return this.AccountId.ToString(); }
        }

        public decimal? NextAlertThreshold
        {
            get { throw new NotImplementedException(); }
        }

        public decimal? LastExecutedAlertThreshold
        {
            get { throw new NotImplementedException(); }
        }


        public int? AlertRuleId
        {
            get { throw new NotImplementedException(); }
        }
    }
}
