using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;
using Vanrise.BEBridge.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Transformation.Entities;

namespace Retail.BusinessEntity.Business
{
    public class AccountSynchronizer : TargetBESynchronizer
    {
        IGenericRuleManager _ruleManager;
        public AccountSynchronizer()
            : base()
        {
            _ruleManager = GetRuleManager(new Guid("E30037DA-29C6-426A-A581-8EB0EDD1D5E3"));
        }
        public override string Name
        {
            get
            {
                return "Accounts";
            }
        }
        public override void InsertBEs(ITargetBESynchronizerInsertBEsContext context)
        {
            if (context.TargetBE == null)
                throw new NullReferenceException("context.TargetBE");
            AccountManager accountManager = new AccountManager();
            foreach (var targetAccount in context.TargetBE)
            {
                SourceAccountData accountData = targetAccount as SourceAccountData;
                long accountId;
                accountManager.TryAddAccount(accountData.Account, out accountId);
                if (accountId > 0 && accountData.IdentificationRulesToInsert != null)
                    foreach (MappingRule mappingRule in accountData.IdentificationRulesToInsert)
                    {
                        mappingRule.Settings.Value = accountId;
                        //var manager = GetRuleManager(mappingRule.DefinitionId);
                        _ruleManager.AddGenericRule(mappingRule);
                    }
            }
        }
        IGenericRuleManager GetRuleManager(Guid ruleDefinitionId)
        {
            GenericRuleDefinitionManager ruleDefinitionManager = new GenericRuleDefinitionManager();
            GenericRuleDefinition ruleDefinition = ruleDefinitionManager.GetGenericRuleDefinition(ruleDefinitionId);

            GenericRuleTypeConfigManager ruleTypeManager = new GenericRuleTypeConfigManager();
            GenericRuleTypeConfig ruleTypeConfig = ruleTypeManager.GetGenericRuleTypeById(ruleDefinition.SettingsDefinition.ConfigId);

            Type managerType = Type.GetType(ruleTypeConfig.RuleManagerFQTN);
            return Activator.CreateInstance(managerType) as IGenericRuleManager;
        }
        public override bool TryGetExistingBE(ITargetBESynchronizerTryGetExistingBEContext context)
        {
            AccountManager accountManager = new AccountManager();
            Account account = accountManager.GetAccountBySourceId(context.SourceBEId as string);
            if (account != null)
            {
                context.TargetBE = new SourceAccountData
                {
                    Account = Serializer.Deserialize<Account>(Serializer.Serialize(account))
                };
                return true;
            }
            return false;
        }
        public override void UpdateBEs(ITargetBESynchronizerUpdateBEsContext context)
        {
            if (context.TargetBE == null)
                throw new NullReferenceException("context.TargetBE");
            AccountManager accountManager = new AccountManager();

            foreach (var target in context.TargetBE)
            {
                SourceAccountData accountData = target as SourceAccountData;

                AccountToEdit editAccount = new AccountToEdit
                {
                    Settings = accountData.Account.Settings,
                    AccountId = accountData.Account.AccountId,
                    Name = accountData.Account.Name,
                    TypeId = accountData.Account.TypeId,
                    SourceId = accountData.Account.SourceId
                };
                accountManager.TryUpdateAccount(editAccount);
            }
        }
    }
}
