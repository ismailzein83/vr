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
    public class AccountBEDefinitionManager
    {
        #region Public Methods

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


        public List<DataRecordGridColumnAttribute> GetAccountGridColumnAttributes(Guid accountBEDefinitionId, long? parentAccountId)
        {
            List<DataRecordGridColumnAttribute> results = new List<DataRecordGridColumnAttribute>();

            AccountGridDefinition accountGridDefinition = this.GetAccountGridDefinition(accountBEDefinitionId);
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
            Account account = new AccountManager().GetAccount(accountId);
            if (account == null)
                throw new NullReferenceException(string.Format("accountId: {0}", accountId));

            return GetAccountViewDefinitionsByAccount(accountBEDefinitionId, account);
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

        private BusinessEntityDefinition GetAccountBusinessEntityDefinition(Guid accountBEDefinitionId)
        {
            BusinessEntityDefinitionManager businessEntityDefinitionManager = new BusinessEntityDefinitionManager();
            return businessEntityDefinitionManager.GetBusinessEntityDefinition(accountBEDefinitionId);
        }
        private AccountBEDefinitionSettings GetAccountBEDefinitionSettings(Guid accountBEDefinitionId)
        {
            var businessEntityDefinition = this.GetAccountBusinessEntityDefinition(accountBEDefinitionId);

            if (businessEntityDefinition == null)
                throw new NullReferenceException(string.Format("businessEntityDefinition Id : {0}", accountBEDefinitionId));

            if (businessEntityDefinition.Settings == null)
                throw new NullReferenceException(string.Format("businessEntityDefinition.Settings Id : {0}", accountBEDefinitionId));

            return businessEntityDefinition.Settings as AccountBEDefinitionSettings;
        }

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
