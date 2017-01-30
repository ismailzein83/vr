using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Business;

namespace Retail.BusinessEntity.Business
{
    public class AccountBEDefinitionManager : IAccountBEDefinitionManager
    {
        #region Public Methods

        public AccountBEDefinitionSettings GetAccountBEDefinitionSettings(Guid accountBEDefinitionId)
        {
            var businessEntityDefinition = new BusinessEntityDefinitionManager().GetBusinessEntityDefinition(accountBEDefinitionId);

            if (businessEntityDefinition == null)
                throw new NullReferenceException(string.Format("businessEntityDefinition Id : {0}", accountBEDefinitionId));

            if (businessEntityDefinition.Settings == null)
                throw new NullReferenceException(string.Format("businessEntityDefinition.Settings Id : {0}", accountBEDefinitionId));

            return businessEntityDefinition.Settings as AccountBEDefinitionSettings;
        }
        public AccountGridDefinition GetAccountGridDefinition(Guid accountBEDefinitionId)
        {
            AccountBEDefinitionSettings accountBEDefinitionSettings = this.GetAccountBEDefinitionSettings(accountBEDefinitionId);

            AccountGridDefinition accountGridDefinition = accountBEDefinitionSettings.GridDefinition;
            if (accountGridDefinition == null)
                throw new NullReferenceException(string.Format("accountGridDefinition for BusinessEntityDefinition Id : {0}", accountBEDefinitionId));

            return accountGridDefinition;
        }
        public List<AccountViewDefinition> GetAccountViewDefinitions(Guid accountBEDefinitionId)
        {
            AccountBEDefinitionSettings accountBEDefinitionSettings = this.GetAccountBEDefinitionSettings(accountBEDefinitionId);

            List<AccountViewDefinition> accountViewDefinitions = accountBEDefinitionSettings.AccountViewDefinitions;
            if (accountViewDefinitions == null)
                throw new NullReferenceException(string.Format("accountViewDefinitions for BusinessEntityDefinition Id : {0}", accountBEDefinitionId));

            return accountViewDefinitions;
        }
        public List<AccountActionDefinition> GetAccountActionDefinitions(Guid accountBEDefinitionId)
        {
            AccountBEDefinitionSettings accountBEDefinitionSettings = this.GetAccountBEDefinitionSettings(accountBEDefinitionId);

            List<AccountActionDefinition> accountActionDefinitions = accountBEDefinitionSettings.ActionDefinitions;
            if (accountActionDefinitions == null)
                throw new NullReferenceException(string.Format("accountActionDefinitions for BusinessEntityDefinition Id : {0}", accountBEDefinitionId));

            return accountActionDefinitions;
        }
        public AccountActionDefinitionSettings GetAccountActionDefinitionSettings(Guid accountBEDefinitionId, Guid actionDefinitionId)
        {
            var accountActionDefinition = GetAccountActionDefinition(accountBEDefinitionId, actionDefinitionId);
            if (accountActionDefinition == null)
                throw new NullReferenceException(string.Format("accountActionDefinition of accountBEDefinitionId {0} nad actionDefinitionId {1}", accountBEDefinitionId, actionDefinitionId));
            return accountActionDefinition.ActionDefinitionSettings;
        }

        public List<DataRecordGridColumnAttribute> GetAccountGridColumnAttributes(Guid accountBEDefinitionId, long? parentAccountId)
        {
            List<DataRecordGridColumnAttribute> results = new List<DataRecordGridColumnAttribute>();

            AccountGridDefinition accountGridDefinition = this.GetAccountGridDefinition(accountBEDefinitionId);
            if (accountGridDefinition.ColumnDefinitions == null)
                throw new NullReferenceException("accountGridDefinition.ColumnDefinitions");

            AccountTypeManager accountTypeManager = new AccountTypeManager();
            IEnumerable<GenericFieldDefinitionInfo> genericFieldDefinitionInfos = accountTypeManager.GetGenericFieldDefinitionsInfo(accountBEDefinitionId);
            if (genericFieldDefinitionInfos == null)
                throw new NullReferenceException("genericFieldDefinitionInfos");

            VRRetailBEVisibilityManager retailBEVisibilityManager = new VRRetailBEVisibilityManager();
            VRRetailBEVisibility retailBEVisibility = retailBEVisibilityManager.GetRetailBEVisibility();
            var visibleGridColumns = retailBEVisibilityManager.GetVisibleGridColumns(retailBEVisibility);

            foreach (AccountGridColumnDefinition itm in accountGridDefinition.ColumnDefinitions)
            {
                if (!IsColumnVisible(retailBEVisibility, visibleGridColumns, itm))
                    continue;

                if (!IsColumnAvailable(accountBEDefinitionId, parentAccountId, itm))
                    continue;

                GenericFieldDefinitionInfo genericFieldDefinitionInfo = genericFieldDefinitionInfos.FindRecord(x => x.Name == itm.FieldName);
                if (genericFieldDefinitionInfo == null)
                    throw new NullReferenceException("genericFieldDefinitionInfo");

                FieldTypeGetGridColumnAttributeContext context = new FieldTypeGetGridColumnAttributeContext();
                context.ValueFieldPath = "FieldValues." + itm.FieldName + ".Value";
                context.DescriptionFieldPath = "FieldValues." + itm.FieldName + ".Description";

                DataRecordGridColumnAttribute attribute = new DataRecordGridColumnAttribute()
                {
                    Attribute = genericFieldDefinitionInfo.FieldType.GetGridColumnAttribute(context),
                    Name = itm.FieldName
                };
                attribute.Attribute.HeaderText = itm.Header;

                results.Add(attribute);
            }

            return results;
        }

        public List<AccountViewDefinition> GetAccountViewDefinitionsByAccount(Guid accountBEDefinitionId, Account account)
        {
            List<AccountViewDefinition> results = new List<AccountViewDefinition>();
            List<AccountViewDefinition> accoutViewDefinitions = this.GetAccountViewDefinitions(accountBEDefinitionId);

            foreach (var itm in accoutViewDefinitions)
            {
                if (IsViewAvailable(itm, account))
                    results.Add(itm);
            }
            return results;
        }
        public List<AccountViewDefinition> GetAccountViewDefinitionsByAccountId(Guid accountBEDefinitionId, long accountId)
        {
            Account account = new AccountBEManager().GetAccount(accountBEDefinitionId, accountId);
            if (account == null)
                throw new NullReferenceException(string.Format("accountId: {0}", accountId));

            return GetAccountViewDefinitionsByAccount(accountBEDefinitionId, account);
        }

        public List<AccountActionDefinition> GetAccountActionDefinitionsByAccount(Guid accountBEDefinitionId, Account account)
        {
            List<AccountActionDefinition> results = new List<AccountActionDefinition>();
            List<AccountActionDefinition> accoutActionDefinitions = this.GetAccountActionDefinitions(accountBEDefinitionId);

            foreach (var itm in accoutActionDefinitions)
            {
                if (IsActionAvailable(itm, account))
                    results.Add(itm);
            }
            return results;
        }

        public IEnumerable<AccountViewDefinitionConfig> GetAccountViewDefinitionSettingsConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<AccountViewDefinitionConfig>(AccountViewDefinitionConfig.EXTENSION_TYPE);
        }
        public IEnumerable<AccountActionDefinitionConfig> GetAccountActionDefinitionSettingsConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<AccountActionDefinitionConfig>(AccountActionDefinitionConfig.EXTENSION_TYPE);
        }

        public AccountActionDefinition GetAccountActionDefinition(Guid accountBEDefinitionId, Guid actionDefinitionId)
        {
            var accountActionDefinitions = GetAccountActionDefinitions(accountBEDefinitionId);
            if (accountActionDefinitions == null)
                throw new NullReferenceException(String.Format("accountActionDefinitions AccountBEDefinitionId '{0}'", accountBEDefinitionId));

            return accountActionDefinitions.FirstOrDefault(x => x.AccountActionDefinitionId == actionDefinitionId);
        }

        #endregion

        #region Private Methods

        private bool IsColumnAvailable(Guid accountBEDefinitionId, long? parentAccountId, AccountGridColumnDefinition gridColumnDefinition)
        {
            bool isRoot = parentAccountId.HasValue ? false : true;

            if (isRoot)
                return gridColumnDefinition.IsAvailableInRoot;

            if (!gridColumnDefinition.IsAvailableInSubAccounts)
                return false;

            if (gridColumnDefinition.SubAccountsAvailabilityCondition != null)
            {
                AccountConditionEvaluationContext context = new AccountConditionEvaluationContext();
                context.Account = new AccountBEManager().GetAccount(accountBEDefinitionId, parentAccountId.Value);
                return gridColumnDefinition.SubAccountsAvailabilityCondition.Evaluate(context);
            }

            return true;
        }
        private bool IsColumnVisible( VRRetailBEVisibility retailBEVisibility, Dictionary<string, VRRetailBEVisibilityAccountDefinitionGridColumns> visibleGridColumns, AccountGridColumnDefinition accountGridColumnDefinition)
        {
            VRRetailBEVisibilityAccountDefinitionGridColumns gridColumn = null;
            if (retailBEVisibility != null && !visibleGridColumns.TryGetValue(accountGridColumnDefinition.FieldName, out gridColumn))
                return false;

            if (gridColumn != null && !string.IsNullOrEmpty(gridColumn.Title))
                accountGridColumnDefinition.Header = gridColumn.Title;

            return true;
        }

        private bool IsViewAvailable(AccountViewDefinition accountViewDefinition, Account account)
        {
            if (accountViewDefinition.AvailabilityCondition != null)
                return accountViewDefinition.AvailabilityCondition.Evaluate(new AccountConditionEvaluationContext() { Account = account });
            return true;
        }

        private bool IsActionAvailable(AccountActionDefinition accountActionDefinition, Account account)
        {
            if (accountActionDefinition.AvailabilityCondition != null)
                return accountActionDefinition.AvailabilityCondition.Evaluate(new AccountConditionEvaluationContext() { Account = account });
            return true;
        }



        #endregion
    }
}
