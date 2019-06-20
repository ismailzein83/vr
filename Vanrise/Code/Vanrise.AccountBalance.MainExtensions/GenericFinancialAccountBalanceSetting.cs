using System;
using Vanrise.AccountBalance.Business;
using Vanrise.AccountBalance.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Notification.Entities;

namespace Vanrise.AccountBalance.MainExtensions
{
    public class GenericFinancialAccountBalanceSetting : AccountTypeExtendedSettings
    {
        public override Guid ConfigId => new Guid("9B248C32-B9A6-4B87-9D0A-8CF7FD6FB11E");

        public override string AccountSelector
        {
            get { return "retail-be-extendedsettings-account-selector"; }
        }
        public GenericFinancialAccountConfiguration Configuration { get; set; }
        public override IAccountManager GetAccountManager()
        {
            return new GenericFinancialAccountBalanceManager(Configuration);
        }

        public override VRActionTargetType GetActionTargetType()
        {
            return new GenericAccountBalanceRuleTargetType { FinancialAccountBEDefinitionId = this.Configuration.FinancialAccountBEDefinitionId };
        }
    }

    public class GenericAccountBalanceRuleTargetType : VRActionTargetType
    {
        public Guid FinancialAccountBEDefinitionId { get; set; }
    }
}
