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
        public Guid AccountBEDefinitionId { get; set; }
        public override string Name
        {
            get
            {
                return "Accounts";
            }
        }

        #region Public Methods
        public override void Initialize(ITargetBESynchronizerInitializeContext context)
        {
            context.InitializationData = new AccountManager().GetCachedAccountsBySourceId();
        }
        public override void InsertBEs(ITargetBESynchronizerInsertBEsContext context)
        {
            if (context.TargetBE == null)
                throw new NullReferenceException("context.TargetBE");
            AccountBEManager accountManager = new AccountBEManager();
            foreach (var targetAccount in context.TargetBE)
            {
                SourceAccountData accountData = targetAccount as SourceAccountData;
                long accountId;
                accountManager.TryAddAccount(GetAccountToInsert(accountData.Account), out accountId);
                if (accountId > 0 && accountData.IdentificationRulesToInsert != null)
                    foreach (MappingRule mappingRule in accountData.IdentificationRulesToInsert)
                    {
                        mappingRule.Settings.Value = accountId;
                        var manager = GetRuleManager(mappingRule.DefinitionId);
                        manager.TryAddGenericRule(mappingRule);
                    }
            }
        }
        public override bool TryGetExistingBE(ITargetBESynchronizerTryGetExistingBEContext context)
        {
            Dictionary<string, Account> existingAccounts = context.InitializationData as Dictionary<string, Account>;
            if (existingAccounts == null)
                throw new NullReferenceException("existingAccounts");
            Account account;
            if (existingAccounts.TryGetValue(context.SourceBEId as string, out account))
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
            AccountBEManager accountManager = new AccountBEManager();

            foreach (var target in context.TargetBE)
            {
                SourceAccountData accountData = target as SourceAccountData;

                AccountToEdit editAccount = new AccountToEdit
                {
                    Settings = accountData.Account.Settings,
                    AccountId = accountData.Account.AccountId,
                    Name = accountData.Account.Name,
                    TypeId = accountData.Account.TypeId,
                    SourceId = accountData.Account.SourceId,
                    AccountBEDefinitionId = this.AccountBEDefinitionId
                };
                accountManager.TryUpdateAccount(editAccount);
            }
        }

        #endregion

        #region Private Methods
        AccountToInsert GetAccountToInsert(Account account)
        {
            return new AccountToInsert
            {
                AccountBEDefinitionId = this.AccountBEDefinitionId,
                Name = account.Name,
                Settings = account.Settings,
                SourceId = account.SourceId,
                StatusId = account.StatusId,
                TypeId = account.TypeId,
                ParentAccountId = account.ParentAccountId
            };
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
        #endregion
    }
}
