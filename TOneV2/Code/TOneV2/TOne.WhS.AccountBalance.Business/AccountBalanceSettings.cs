using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.AccountBalance.Entities;
using Vanrise.AccountBalance.Entities;
using Vanrise.Notification.Entities;

namespace TOne.WhS.AccountBalance.Business
{
    public abstract class AccountBalanceSettings : AccountTypeExtendedSettings
    {
        public override string AccountSelector { get { return "whs-accountbalance-account-selector"; } }

        public abstract bool IsApplicableToCustomer { get; }

        public abstract bool IsApplicableToSupplier { get; }

        public virtual string RuntimeEditor { get; set; }

        public override IAccountManager GetAccountManager() { return new AccountBalanceManager(); }

        public override VRActionTargetType GetActionTargetType()
        {
            return new CustomerAccountBalanceRuleTargetType();
        }
    }

    public class CustomerAccountBalanceRuleTargetType : VRActionTargetType
    {

    }
}