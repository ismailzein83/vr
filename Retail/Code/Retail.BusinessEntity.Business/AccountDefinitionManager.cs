﻿using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace Retail.BusinessEntity.Business
{
    public class AccountDefinitionManager
    {
        #region Public Methods

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

        public List<AccountViewDefinition> GetAccountViewDefinitionsByAccount(long accountId)
        {
            return GetAccountViewDefinitions();
        }

        public IEnumerable<AccountViewDefinitionSettingsConfig> GetAccountViewDefinitionSettingsConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<AccountViewDefinitionSettingsConfig>(AccountViewDefinitionSettingsConfig.EXTENSION_TYPE);
        }

        #endregion


        #region Private Methods

        private bool IsColumnAvailable(long? parentAccountId, AccountGridColumnDefinition gridColumnDefinition)
        {
            bool isRoot = parentAccountId.HasValue ? false : true;

            if(isRoot)
                return gridColumnDefinition.IsAvailableInRoot;

            if (!gridColumnDefinition.IsAvailableInSubAccounts)
                return false;

            if (gridColumnDefinition.SubAccountsAvailabilityCondition == null)
                return true;

            AccountConditionEvaluationContext context = new AccountConditionEvaluationContext();
            context.Account = new AccountManager().GetAccount(parentAccountId.Value);

            return gridColumnDefinition.SubAccountsAvailabilityCondition.Evaluate(context);
        }

        #endregion
    }
}
