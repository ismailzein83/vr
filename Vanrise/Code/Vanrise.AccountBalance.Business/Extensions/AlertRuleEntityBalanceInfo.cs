using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.Notification.Entities;
using Vanrise.Common;

namespace Vanrise.AccountBalance.Business
{
    public class AlertRuleEntityBalanceInfo : IVREntityBalanceInfo
    {
        public AlertRuleEntityBalanceInfo(LiveBalance liveBalance)
        {
            liveBalance.ThrowIfNull("liveBalance");
            this.LiveBalance = liveBalance;
        }

        public LiveBalance LiveBalance { get; set; }

        public string EntityId
        {
            get { return this.LiveBalance.AccountId; }
        }

        string _accountName;

        public string EntityName
        {
            get
            {
                if (_accountName == null)
                    _accountName = new AccountManager().GetAccountName(this.LiveBalance.AccountTypeId, this.LiveBalance.AccountId);
                return _accountName;
            }
        }

        public decimal CurrentBalance
        {
            get { return this.LiveBalance.CurrentBalance; }
        }

        public int? AlertRuleId
        {
            get { return this.LiveBalance.AlertRuleID; }
        }

        public decimal? NextAlertThreshold
        {
            get { return this.LiveBalance.NextThreshold; }
        }

        public decimal? LastExecutedAlertThreshold
        {
            get { return this.LiveBalance.LastExecutedThreshold; }
        }

        public VRBalanceActiveAlertInfo ActiveAlertsInfo
        {
            get { return this.LiveBalance.LiveBalanceActiveAlertsInfo; }
        }
    }
}
