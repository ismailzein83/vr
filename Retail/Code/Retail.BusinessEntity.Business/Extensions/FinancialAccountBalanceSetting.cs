using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.Notification.Entities;

namespace Retail.BusinessEntity.Business
{
    public class FinancialAccountBalanceSetting : SubscriberAccountBalanceSetting
    {
        public override Guid ConfigId
        {
            get { return new Guid("6A33AFF5-C8D0-41BA-906D-3F9CBB8A7D3E"); }
        }

        public override string AccountSelector
        {
            get { return "retail-be-extendedsettings-financialaccount-selector"; }
        }

        public override IAccountManager GetAccountManager()
        {
            return new FinancialAccountBalanceManager(this.AccountBEDefinitionId);
        }

        public override VRActionTargetType GetActionTargetType()
        {
            return new RetailAccountBalanceRuleTargetType { AccountBEDefinitionId = this.AccountBEDefinitionId};
        }

    }
}
