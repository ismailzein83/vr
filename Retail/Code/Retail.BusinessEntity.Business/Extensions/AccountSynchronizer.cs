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

        public List<AccountSynchronizerInsertHandler> InsertHandlers { get; set; }

        #region Public Methods
        public override void Initialize(ITargetBESynchronizerInitializeContext context)
        {
            context.InitializationData = new AccountBEManager().GetCachedAccountsBySourceId(this.AccountBEDefinitionId);
        }
        public override void InsertBEs(ITargetBESynchronizerInsertBEsContext context)
        {
            if (context.TargetBE == null)
                throw new NullReferenceException("context.TargetBE");

            foreach (var targetAccount in context.TargetBE)
            {
                SourceAccountData accountData = targetAccount as SourceAccountData;
                AddAccount(accountData, null);
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
                if (accountData.Account.AccountId == 0)
                    continue;
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
        static AccountBEManager s_accountManager = new AccountBEManager();
        void AddAccount(SourceAccountData accountData, long? parentAccountId)
        {
            List<AccountSynchronizerInsertHandler> handlersToExecute = GetHandlersToExecute(accountData.Account);
            ApplyHandlersPreInsert(accountData.Account, handlersToExecute);
            long accountId;
            s_accountManager.TryAddAccount(GetAccountToInsert(accountData.Account), out accountId, true);
            if (accountId > 0)
            {
                AccountBEManager accountManager = new AccountBEManager();
                accountData.Account.AccountId = accountId;
                var cachedAccounts = accountManager.GetCachedAccounts(this.AccountBEDefinitionId);
                if (!cachedAccounts.ContainsKey((accountId)))
                    cachedAccounts.Add(accountId, accountData.Account);

                if (accountData.IdentificationRulesToInsert != null)
                    foreach (MappingRule mappingRule in accountData.IdentificationRulesToInsert)
                    {
                        mappingRule.Settings.Value = accountId;
                        var manager = GetRuleManager(mappingRule.DefinitionId);
                        manager.TryAddGenericRule(mappingRule);
                    }
                if (accountData.ChildrenAccounts != null && accountData.ChildrenAccounts.Count > 0)
                {
                    foreach (var childAccount in accountData.ChildrenAccounts)
                    {
                        childAccount.Account.ParentAccountId = accountId;
                        AddAccount(childAccount, accountId);
                    }
                }
                ApplyHandlersPostInsert(accountData.Account, handlersToExecute);
            }
        }

        private void ApplyHandlersPreInsert(Account account, List<AccountSynchronizerInsertHandler> handlersToExecute)
        {
            if (handlersToExecute != null)
            {
                foreach (var handler in handlersToExecute)
                {
                    var context = new AccountSynchronizerInsertHandlerPreInsertContext
                    {
                        AccountBEDefinitionId = this.AccountBEDefinitionId,
                        Account = account
                    };
                    handler.Settings.OnPreInsert(context);
                }
            }
        }

        private void ApplyHandlersPostInsert(Account account, List<AccountSynchronizerInsertHandler> handlersToExecute)
        {
            if (handlersToExecute != null)
            {
                foreach (var handler in handlersToExecute)
                {
                    var context = new AccountSynchronizerInsertHandlerPostInsertContext
                    {
                        AccountBEDefinitionId = this.AccountBEDefinitionId,
                        Account = account
                    };
                    handler.Settings.OnPostInsert(context);
                }
            }
        }

        private List<AccountSynchronizerInsertHandler> GetHandlersToExecute(Account account)
        {
            List<AccountSynchronizerInsertHandler> handlersToExecute = null;
            if (this.InsertHandlers != null)
            {
                foreach (var handler in this.InsertHandlers)
                {
                    if (handler.AccountCondition != null)
                    {
                        if (!s_accountManager.EvaluateAccountCondition(account, handler.AccountCondition))
                            continue;
                    }
                    if (handlersToExecute == null)
                        handlersToExecute = new List<AccountSynchronizerInsertHandler>();
                    handlersToExecute.Add(handler);
                }
            }
            return handlersToExecute;
        }

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
                ParentAccountId = account.ParentAccountId,
                CreatedTime = account.CreatedTime
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

        #region Private Classes

        private class AccountSynchronizerInsertHandlerPreInsertContext : IAccountSynchronizerInsertHandlerPreInsertContext
        {
            public Guid AccountBEDefinitionId
            {
                get;
                set;
            }

            public Account Account
            {
                get;
                set;
            }
        }

        private class AccountSynchronizerInsertHandlerPostInsertContext : IAccountSynchronizerInsertHandlerPostInsertContext
        {
            public Guid AccountBEDefinitionId
            {
                get;
                set;
            }

            public Account Account
            {
                get;
                set;
            }
        }


        #endregion
    }
}
