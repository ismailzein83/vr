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
    public class AccountDefinitionManager
    {
        #region Public Methods

        public BusinessEntityDefinition GetAccountBusinessEntityDefinition(Guid accountDefinitionId)
        {
            BusinessEntityDefinitionManager businessEntityDefinitionManager = new BusinessEntityDefinitionManager();
            return businessEntityDefinitionManager.GetBusinessEntityDefinition(accountDefinitionId);
        }
        public AccountBEDefinitionSettings GetAccountBEDefinitionSettings(Guid accountDefinitionId)
        {
            var businessEntityDefinition = this.GetAccountBusinessEntityDefinition(accountDefinitionId);

            if (businessEntityDefinition == null)
                throw new NullReferenceException(string.Format("businessEntityDefinition Id : {0}", accountDefinitionId));

            if (businessEntityDefinition.Settings == null)
                throw new NullReferenceException(string.Format("businessEntityDefinition.Settings Id : {0}", accountDefinitionId));

            return businessEntityDefinition.Settings as AccountBEDefinitionSettings;
        }

        public AccountGridDefinition GetAccountGridDefinition(Guid accountDefinitionId)
        {
            AccountBEDefinitionSettings accountBEDefinitionSettings = this.GetAccountBEDefinitionSettings(accountDefinitionId);

            AccountGridDefinition accountGridDefinition = accountBEDefinitionSettings.GridDefinition;
            if (accountGridDefinition == null)
                throw new NullReferenceException(string.Format("accountGridDefinition for BusinessEntityDefintion Id : {0}", accountDefinitionId));

            return accountGridDefinition;
        }
        public List<AccountViewDefinition> GetAccountViewDefinitions(Guid accountDefinitionId)
        {
            AccountBEDefinitionSettings accountBEDefinitionSettings = this.GetAccountBEDefinitionSettings(accountDefinitionId);

            List<AccountViewDefinition> accountViewDefinitions = accountBEDefinitionSettings.AccountViewDefinitions;
            if (accountViewDefinitions == null)
                throw new NullReferenceException("accountViewDefinitions for BusinessEntityDefintion Id : {0}");

            return accountViewDefinitions;
        }

        public List<DataRecordGridColumnAttribute> GetAccountGridColumnAttributes(long? parentAccountId)
        {
            List<DataRecordGridColumnAttribute> results = new List<DataRecordGridColumnAttribute>();

            ConfigManager configManager = new ConfigManager();
            AccountGridDefinition accountGridDefinition = configManager.GetAccountGridDefinition();
            if (accountGridDefinition.ColumnDefinitions == null)
                throw new NullReferenceException("accountGridDefinition.ColumnDefinitions");

            AccountTypeManager accountTypeManager = new AccountTypeManager();
            IEnumerable<GenericFieldDefinitionInfo> genericFieldDefinitionInfos = accountTypeManager.GetGenericFieldDefinitionsInfo();
            if (genericFieldDefinitionInfos == null)
                throw new NullReferenceException("genericFieldDefinitionInfos");

            foreach (AccountGridColumnDefinition itm in accountGridDefinition.ColumnDefinitions)
            {
                if (!IsColumnAvailable(parentAccountId, itm))
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

        public List<AccountViewDefinition> GetAccountViewDefinitions()
        {
            ConfigManager configManager = new ConfigManager();
            return configManager.GetAccountViewDefinitions();
        }
        public List<AccountViewDefinition> GetAccountViewDefinitionsByAccount(Account account)
        {
            List<AccountViewDefinition> results = new List<AccountViewDefinition>();
            List<AccountViewDefinition> accoutViewDefinitions = this.GetAccountViewDefinitions();

            foreach (var itm in accoutViewDefinitions)
            {
                if (IsViewAvailable(itm, account))
                    results.Add(itm);
            }
            return results;
        }
        public List<AccountViewDefinition> GetAccountViewDefinitionsByAccountId(long accountId)
        {
            Account account = new AccountManager().GetAccount(accountId);
            if (account == null)
                throw new NullReferenceException(string.Format("accountId: {0}", accountId));

            return GetAccountViewDefinitionsByAccount(account);
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

        #endregion

        #region Private Methods

        private bool IsColumnAvailable(long? parentAccountId, AccountGridColumnDefinition gridColumnDefinition)
        {
            bool isRoot = parentAccountId.HasValue ? false : true;

            if (isRoot)
                return gridColumnDefinition.IsAvailableInRoot;

            if (!gridColumnDefinition.IsAvailableInSubAccounts)
                return false;

            if (gridColumnDefinition.SubAccountsAvailabilityCondition != null)
            {
                AccountConditionEvaluationContext context = new AccountConditionEvaluationContext();
                context.Account = new AccountManager().GetAccount(parentAccountId.Value);
                return gridColumnDefinition.SubAccountsAvailabilityCondition.Evaluate(context);
            }

            return true;
        }

        private bool IsViewAvailable(AccountViewDefinition accountViewDefinition, Account account)
        {
            if (accountViewDefinition.AvailabilityCondition != null)
                return accountViewDefinition.AvailabilityCondition.Evaluate(new AccountConditionEvaluationContext() { Account = account });
            
            return true;
        }

        #endregion
    }
}
