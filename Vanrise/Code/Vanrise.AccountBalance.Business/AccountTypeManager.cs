using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Security.Business;

namespace Vanrise.AccountBalance.Business
{
    public class AccountTypeManager : IAccountTypeManager
    {
        #region Ctor/Properties

        Vanrise.Common.Business.VRComponentTypeManager _vrComponentTypeManager = new Common.Business.VRComponentTypeManager();
        static SecurityManager s_securityManager = new SecurityManager();

        #endregion

        #region Public Methods

        public string GetAccountSelector(Guid accountTypeId)
        {
            var accountTypeSettings = GetAccountTypeSettings(accountTypeId);
            if (accountTypeSettings.ExtendedSettings == null)
                throw new NullReferenceException("accountTypeSettings.ExtendedSettings");
            return accountTypeSettings.ExtendedSettings.AccountSelector;
        }
        public Guid? GetInvToAccBalanceRelationId(Guid accountTypeId)
        {
            var accountTypeSettings = GetAccountTypeSettings(accountTypeId);
            accountTypeSettings.ThrowIfNull("accountTypeSettings", accountTypeId);
            return accountTypeSettings.InvToAccBalanceRelationId;
        }
        public string GetAccountTypeName(Guid accountTypeId)
        {
            var accountType = _vrComponentTypeManager.GetComponentType<AccountTypeSettings, AccountType>(accountTypeId);
            accountType.ThrowIfNull("accountType", accountTypeId);
            return accountType.Name;
        }

        public IEnumerable<AccountTypeExtendedSettingsConfig> GetAccountBalanceExtendedSettingsConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<AccountTypeExtendedSettingsConfig>(AccountTypeExtendedSettingsConfig.EXTENSION_TYPE);
        }
        public IAccountManager GetAccountManager(Guid accountTypeId)
        {
            var accountTypeSettings = GetAccountTypeSettings(accountTypeId);
            if (accountTypeSettings.ExtendedSettings == null)
                throw new NullReferenceException("accountTypeSettings.ExtendedSettings");
            return accountTypeSettings.ExtendedSettings.GetAccountManager();
        }

        public IEnumerable<BalanceAccountTypeInfo> GetAccountTypeInfo(AccountTypeInfoFilter filter)
        {
            Func<AccountType, bool> filterExpression = null;
            if (filter != null)
            {
                filterExpression = (item) => CheckIfFilterIsMatch(item, filter.Filters);
            }

            return _vrComponentTypeManager.GetComponentTypes<AccountTypeSettings, AccountType>().MapRecords(AccountTypeInfoMapper, filterExpression);
        }

        public IEnumerable<AccountType> GetAccountTypes(AccountTypeFilter accountTypeFilter)
        {
            Func<AccountType, bool> filterExpression = (accountType) =>
                {
                    if (accountTypeFilter == null)
                        return true;

                    if (accountTypeFilter.Filters != null && !CheckIfFiltersAreMatched(accountType, accountTypeFilter.Filters))
                        return false;

                    return true;
                };

            return _vrComponentTypeManager.GetComponentTypes<AccountTypeSettings, AccountType>().Where(filterExpression);
        }
        public IEnumerable<AccountType> GetAccountTypesByExtendedSettingsType<T>() where T : AccountTypeExtendedSettings
        {
            Func<AccountType, bool> filterExpression = (accountType) =>
            {
                var extendedSettings = accountType.Settings.ExtendedSettings as T;
                if (extendedSettings == null)
                    return false;
                return true;
            };
            return _vrComponentTypeManager.GetComponentTypes<AccountTypeSettings, AccountType>().Where(filterExpression);
        }

        public BalancePeriodSettings GetBalancePeriodSettings(Guid accountTypeId)
        {
            var accountTypeSettings = GetAccountTypeSettings(accountTypeId);
            return accountTypeSettings.BalancePeriodSettings;
        }
        public AccountUsagePeriodSettings GetAccountUsagePeriodSettings(Guid accountTypeId)
        {
            var accountTypeSettings = GetAccountTypeSettings(accountTypeId);
            return accountTypeSettings.AccountUsagePeriodSettings;
        }
        public TimeSpan GetTimeOffset(Guid accountTypeId)
        {
            var accountTypeSettings = GetAccountTypeSettings(accountTypeId);
            return accountTypeSettings.TimeOffset;
        }
        public AccountTypeSettings GetAccountTypeSettings(Guid accountTypeId)
        {
            return _vrComponentTypeManager.GetComponentTypeSettings<AccountTypeSettings>(accountTypeId);
        }

        public IEnumerable<AccountTypeSourcesConfig> GetAccountTypeSourceSettingsConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<AccountTypeSourcesConfig>(AccountTypeSourcesConfig.EXTENSION_TYPE);
        }
        public T GetAccountTypeExtendedSettings<T>(Guid accountTypeId) where T : AccountTypeExtendedSettings
        {
            AccountTypeSettings accountTypeSettings = GetAccountTypeSettings(accountTypeId);
            accountTypeSettings.ThrowIfNull("accountTypeSettings", accountTypeId);
            return accountTypeSettings.ExtendedSettings.CastWithValidate<T>("accountTypeSettings.ExtendedSettings", accountTypeId);
        }

        public bool DoesUserHaveViewAccess(int userId, List<Guid> AccountTypeIds)
        {
            foreach (var a in AccountTypeIds)
            {
                if (DoesUserHaveViewAccess(userId, a))
                    return true;
            }
            return false;
        }
        public bool DoesUserHaveViewAccess(Guid accountTypeId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            var accountTypeSettings = GetAccountTypeSettings(accountTypeId);
            return DoesUserHaveViewAccess(userId, accountTypeSettings);
        }
        public bool DoesUserHaveViewAccess(int userId, Guid accountTypeId)
        {
            var accountTypeSettings = GetAccountTypeSettings(accountTypeId);
            return DoesUserHaveViewAccess(userId, accountTypeSettings);
        }
        public bool DoesUserHaveViewAccess(int userId, AccountTypeSettings accountTypeSettings)
        {
            if (accountTypeSettings.Security != null && accountTypeSettings.Security.ViewRequiredPermission != null)
                return s_securityManager.IsAllowed(accountTypeSettings.Security.ViewRequiredPermission, userId);
            else
                return true;
        }
        public bool DoesUserHaveAddAccess(Guid accountBeDefinitionId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return DoesUserHaveAddAccess(userId, accountBeDefinitionId);
        }
        public bool DoesUserHaveAddAccess(int userId, Guid accountBeDefinitionId)
        {
            var accountBEDefinitionSettings = GetAccountTypeSettings(accountBeDefinitionId);
            if (accountBEDefinitionSettings != null && accountBEDefinitionSettings.Security != null && accountBEDefinitionSettings.Security.AddRequiredPermission != null)
                return s_securityManager.IsAllowed(accountBEDefinitionSettings.Security.AddRequiredPermission, userId);
            else
                return true;
        }
        public IEnumerable<AccountBalanceFieldDefinition> GetAccountTypeSourceFields(AccountBalanceFieldSource source, AccountTypeSettings accountTypeSettings)
        {
            List<AccountBalanceFieldDefinition> fields = new List<AccountBalanceFieldDefinition>();
            return source.Settings.GetFieldDefinitions(new AccountBalanceFieldSourceGetFieldDefinitionsContext { AccountTypeSettings = accountTypeSettings });
        }
        public Dictionary<Guid, IEnumerable<AccountBalanceFieldDefinition>> GetAccountTypeSourcesFields(List<AccountBalanceFieldSource> sources, AccountTypeSettings accountTypeSettings)
        {
            Dictionary<Guid, IEnumerable<AccountBalanceFieldDefinition>> fieldsBySourceId = new Dictionary<Guid, IEnumerable<AccountBalanceFieldDefinition>>();
            if (sources != null)
            {
                foreach (var source in sources)
                {
                    if (!fieldsBySourceId.ContainsKey(source.AccountBalanceFieldSourceId))
                    {
                        var fields = GetAccountTypeSourceFields(source, accountTypeSettings);
                        if (fields != null)
                        {
                            fieldsBySourceId.Add(source.AccountBalanceFieldSourceId, fields);
                        }
                    }
                }
            }
            return fieldsBySourceId;
        }

        public IEnumerable<GridColumnAttribute> ConvertToGridColumnAttribute(Guid accountTypeId)
        {

            var accountTypeSettings = GetAccountTypeSettings(accountTypeId);
            List<GridColumnAttribute> gridColumnAttributes = null;
            if (accountTypeSettings.AccountBalanceGridSettings != null && accountTypeSettings.AccountBalanceGridSettings.GridColumns != null)
            {
                gridColumnAttributes = new List<GridColumnAttribute>();
                foreach (var column in accountTypeSettings.AccountBalanceGridSettings.GridColumns)
                {

                    var source = accountTypeSettings.Sources.FirstOrDefault(x => x.AccountBalanceFieldSourceId == column.SourceId);
                    if (source != null)
                    {
                        var sourceFields = source.Settings.GetFieldDefinitions(new AccountBalanceFieldSourceGetFieldDefinitionsContext { AccountTypeSettings = accountTypeSettings });
                        if (sourceFields != null)
                        {
                            var matchField = sourceFields.FirstOrDefault(x => x.Name == column.FieldName);
                            if (matchField.FieldType == null)
                                throw new NullReferenceException(string.Format("{0} is not mapped to field type.", matchField.Name));
                           
                            FieldTypeGetGridColumnAttributeContext context = new FieldTypeGetGridColumnAttributeContext();
                            context.ValueFieldPath = "Items." + column.FieldName + ".Value";
                            context.DescriptionFieldPath = "Items." + column.FieldName + ".Description";

                            var gridAttribute = matchField.FieldType.GetGridColumnAttribute(context);
                          //  gridAttribute.Field = matchField.Name;
                            gridAttribute.Tag = matchField.Name;
                            gridAttribute.HeaderText = column.UseEmptyHeader ? "" : column.Title;
                            gridAttribute.GridColCSSClassValue = column.GridColCSSValue;
                            //if (column.GridColumnSettings != null)
                            //{
                            //    gridAttribute.WidthFactor = GridColumnWidthFactorConstants.GetColumnWidthFactor(column.GridColumnSettings);
                            //    if (!gridAttribute.WidthFactor.HasValue)
                            //        gridAttribute.FixedWidth = column.GridColumnSettings.FixedWidth;
                            //}
                            gridColumnAttributes.Add(gridAttribute);
                        }
                    }
                }
            }
            return gridColumnAttributes;

        }

        public IEnumerable<Guid> GetAllowedBillingTransactionTypeIds(Guid accountTypeId)
        {
            AccountTypeSettings accountTypeSettings = GetAccountTypeSettings(accountTypeId);
            return (accountTypeSettings != null) ? accountTypeSettings.AllowedBillingTransactionTypeIds : null;
        }

        public bool ShouldGroupUsagesByTransactionType(Guid accountTypeId)
        {
            AccountTypeSettings accountTypeSettings = GetAccountTypeSettings(accountTypeId);
            accountTypeSettings.ThrowIfNull("accountTypeSettings", accountTypeId);
            return accountTypeSettings.ShouldGroupUsagesByTransactionType;
        }
        public bool ShouldExcludeUsageFromStatement(Guid accountTypeId)
        {
            AccountTypeSettings accountTypeSettings = GetAccountTypeSettings(accountTypeId);
            accountTypeSettings.ThrowIfNull("accountTypeSettings", accountTypeId);
            return accountTypeSettings.ExcludeUsageFromStatement;
        }
        #endregion

        #region Private Methods

        private bool CheckIfFilterIsMatch(AccountType accountType, List<IAccountTypeInfoFilter> filters)
        {
            AccountTypeInfoFilterContext context = new AccountTypeInfoFilterContext { AccountType = accountType };
            foreach (var filter in filters)
            {
                if (!filter.IsMatched(context))
                    return false;
            }
            return true;
        }

        private bool CheckIfFiltersAreMatched(AccountType accountType, List<IAccountTypeExtendedSettingsFilter> filters)
        {
            AccountTypeExtendedSettingsFilterContext context = new AccountTypeExtendedSettingsFilterContext { AccountType = accountType };
            foreach (var filter in filters)
            {
                if (!filter.IsMatched(context))
                    return false;
            }
            return true;
        }

        #endregion

        #region Mappers

        private BalanceAccountTypeInfo AccountTypeInfoMapper(AccountType accountType)
        {
            string editor = null;
            if (accountType != null && accountType.Settings != null && accountType.Settings.ExtendedSettings != null)
                editor = accountType.Settings.ExtendedSettings.AccountSelector;
            return new BalanceAccountTypeInfo
            {
                Id = accountType.VRComponentTypeId,
                Name = accountType.Name,
                Editor = editor
            };
        }

        #endregion
    }
}
