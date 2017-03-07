﻿using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.Security.Business;

namespace Retail.BusinessEntity.Business
{
    public class AccountBEDefinitionManager : IAccountBEDefinitionManager
    {
        #region Ctor/Fields

        static BusinessEntityDefinitionManager s_businessEntityDefinitionManager = new BusinessEntityDefinitionManager();

        static SecurityManager s_securityManager = new SecurityManager();

        #endregion

        #region Public Methods

        public AccountBEDefinitionSettings GetAccountBEDefinitionSettingsWithHidden(Guid accountBEDefinitionId)
        {
            var businessEntityDefinition = s_businessEntityDefinitionManager.GetBusinessEntityDefinition(accountBEDefinitionId);

            if (businessEntityDefinition == null)
                throw new NullReferenceException(string.Format("businessEntityDefinition Id : {0}", accountBEDefinitionId));

            if (businessEntityDefinition.Settings == null)
                throw new NullReferenceException(string.Format("businessEntityDefinition.Settings Id : {0}", accountBEDefinitionId));

            return businessEntityDefinition.Settings as AccountBEDefinitionSettings;
        }

        private struct GetAccountBEDefinitionSettingsCacheName
        {
            public Guid AccountBEDefinitionId { get; set; }
        }
        public AccountBEDefinitionSettings GetAccountBEDefinitionSettings(Guid accountBEDefinitionId)
        {
            var cacheName = new GetAccountBEDefinitionSettingsCacheName { AccountBEDefinitionId = accountBEDefinitionId };
            return s_businessEntityDefinitionManager.GetCachedOrCreate(cacheName, () =>
            {
                var businessEntityDefinition = s_businessEntityDefinitionManager.GetBusinessEntityDefinition(accountBEDefinitionId);

                if (businessEntityDefinition == null)
                    throw new NullReferenceException(string.Format("businessEntityDefinition Id : {0}", accountBEDefinitionId));

                if (businessEntityDefinition.Settings == null)
                    throw new NullReferenceException(string.Format("businessEntityDefinition.Settings Id : {0}", accountBEDefinitionId));

                var businessEntityDefinitionSettings = businessEntityDefinition.Settings as AccountBEDefinitionSettings;

                VRRetailBEVisibilityManager retailBEVisibilityManager = new VRRetailBEVisibilityManager();
                AccountBEDefinitionSettings accountBEDefinitionSettings = new AccountBEDefinitionSettings();
                accountBEDefinitionSettings.StatusBEDefinitionId = businessEntityDefinitionSettings.StatusBEDefinitionId;

                //GridColumns
                Dictionary<string, VRRetailBEVisibilityAccountDefinitionGridColumns> visibleGridColumnsByFieldName;

                if (businessEntityDefinitionSettings.GridDefinition != null && businessEntityDefinitionSettings.GridDefinition.ColumnDefinitions != null)
                {
                    if (retailBEVisibilityManager.ShouldApplyGridColumnsVisibility(out visibleGridColumnsByFieldName))
                    {
                        accountBEDefinitionSettings.GridDefinition = new AccountGridDefinition();
                        accountBEDefinitionSettings.GridDefinition.ColumnDefinitions = new List<AccountGridColumnDefinition>();

                        foreach (var gridColumn in businessEntityDefinitionSettings.GridDefinition.ColumnDefinitions)
                        {
                            if (IsColumnVisible(visibleGridColumnsByFieldName, gridColumn))
                                accountBEDefinitionSettings.GridDefinition.ColumnDefinitions.Add(gridColumn);
                        }
                    }
                    else
                    {
                        accountBEDefinitionSettings.GridDefinition = businessEntityDefinitionSettings.GridDefinition;
                    }

                }

                //Views
                Dictionary<Guid, VRRetailBEVisibilityAccountDefinitionView> visibleViewsById;

                if (businessEntityDefinitionSettings.AccountViewDefinitions != null)
                {
                    if (retailBEVisibilityManager.ShouldApplyViewsVisibility(out visibleViewsById))
                    {
                        accountBEDefinitionSettings.AccountViewDefinitions = new List<AccountViewDefinition>();

                        foreach (var view in businessEntityDefinitionSettings.AccountViewDefinitions)
                        {
                            if (IsViewVisible(visibleViewsById, view))
                                accountBEDefinitionSettings.AccountViewDefinitions.Add(view);
                        }
                    }
                    else
                    {
                        accountBEDefinitionSettings.AccountViewDefinitions = businessEntityDefinitionSettings.AccountViewDefinitions;
                    }
                }


                //Actions
                Dictionary<Guid, VRRetailBEVisibilityAccountDefinitionAction> visibleActionsById;

                if (businessEntityDefinitionSettings.ActionDefinitions != null)
                {
                    accountBEDefinitionSettings.ActionDefinitions = new List<AccountActionDefinition>();
                    if (retailBEVisibilityManager.ShouldApplyActionsVisibility(out visibleActionsById))
                    {
                        foreach (var action in businessEntityDefinitionSettings.ActionDefinitions)
                        {

                            if (IsActionVisible(visibleActionsById, action))
                                accountBEDefinitionSettings.ActionDefinitions.Add(action);
                        }
                    }
                    else
                        accountBEDefinitionSettings.ActionDefinitions = businessEntityDefinitionSettings.ActionDefinitions;
                }

                //Extra Fields
                // Dictionary<Guid, VRRetailBEVisibilityAccountDefinitionAction> visibleExtraFieldsById;

                accountBEDefinitionSettings.AccountExtraFieldDefinitions = businessEntityDefinitionSettings.AccountExtraFieldDefinitions;


                //GridColumnsExportExcel
                if (businessEntityDefinitionSettings.GridDefinition != null && businessEntityDefinitionSettings.GridDefinition.ExportColumnDefinitions != null)
                {
                    accountBEDefinitionSettings.GridDefinition.ExportColumnDefinitions = businessEntityDefinitionSettings.GridDefinition.ExportColumnDefinitions;
                }

                if (businessEntityDefinitionSettings.Security != null)
                    accountBEDefinitionSettings.Security = businessEntityDefinitionSettings.Security;
                return accountBEDefinitionSettings;
            });
        }

        public List<AccountExtraFieldDefinition> GetAccountExtraFieldDefinitions(Guid accountBEDefinitionId)
        {
            AccountBEDefinitionSettings accountBEDefinitionSettings = this.GetAccountBEDefinitionSettings(accountBEDefinitionId);

            return accountBEDefinitionSettings.AccountExtraFieldDefinitions;
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

            foreach (AccountGridColumnDefinition itm in accountGridDefinition.ColumnDefinitions)
            {
                if (!IsColumnAvailable(accountBEDefinitionId, parentAccountId, itm))
                    continue;

                GenericFieldDefinitionInfo genericFieldDefinitionInfo = genericFieldDefinitionInfos.FindRecord(x => x.Name == itm.FieldName);
                if (genericFieldDefinitionInfo == null)
                    continue; // throw new NullReferenceException("genericFieldDefinitionInfo");

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

        public List<DataRecordGridColumnAttribute> GetAccountGridColumnAttributesExportExcel(Guid accountBEDefinitionId)
        {
            List<DataRecordGridColumnAttribute> results = new List<DataRecordGridColumnAttribute>();

            AccountGridDefinition accountGridDefinition = this.GetAccountGridDefinition(accountBEDefinitionId);
            if (accountGridDefinition.ExportColumnDefinitions == null)
                throw new NullReferenceException("accountGridDefinition.ExportColumnDefinitions");

            AccountTypeManager accountTypeManager = new AccountTypeManager();
            IEnumerable<GenericFieldDefinitionInfo> genericFieldDefinitionInfos = accountTypeManager.GetGenericFieldDefinitionsInfo(accountBEDefinitionId);
            if (genericFieldDefinitionInfos == null)
                throw new NullReferenceException("genericFieldDefinitionInfos");

            foreach (AccountGridExportColumnDefinition itm in accountGridDefinition.ExportColumnDefinitions)
            {

                GenericFieldDefinitionInfo genericFieldDefinitionInfo = genericFieldDefinitionInfos.FindRecord(x => x.Name == itm.FieldName);
                if (genericFieldDefinitionInfo == null)
                    continue; // throw new NullReferenceException("genericFieldDefinitionInfo");

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
        public AccountActionDefinition GetAccountActionDefinition(Guid accountBEDefinitionId, Guid actionDefinitionId)
        {
            var accountActionDefinitions = GetAccountActionDefinitions(accountBEDefinitionId);
            if (accountActionDefinitions == null)
                throw new NullReferenceException(String.Format("accountActionDefinitions AccountBEDefinitionId '{0}'", accountBEDefinitionId));

            return accountActionDefinitions.FirstOrDefault(x => x.AccountActionDefinitionId == actionDefinitionId);
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
        public IEnumerable<AccountExtraFieldDefinitionConfig> GetAccountExtraFieldDefinitionSettingsConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<AccountExtraFieldDefinitionConfig>(AccountExtraFieldDefinitionConfig.EXTENSION_TYPE);
        }
        public IEnumerable<AccountActionDefinitionInfo> GetAccountActionDefinitionsInfo(Guid accountBEDefinitionId, AccountActionDefinitionInfoFilter filter)
        {
            var accountBEActions = GetAccountActionDefinitions(accountBEDefinitionId);
            Func<AccountActionDefinition, bool> filterExpression = null;
            if (filter != null)
            {
                filterExpression = (accountActionDefinition) =>
                {
                    if (filter.VisibleInBalanceAlertRule.HasValue && accountActionDefinition.VisibleInBalanceAlertRule != filter.VisibleInBalanceAlertRule.Value)
                        return false;
                    return true;
                };
            }
            return accountBEActions.MapRecords(AccountActionDefinitionInfoMapper, filterExpression).OrderBy(x => x.Name);
        }


    

        #endregion

        #region Security
        public bool DoesUserHaveViewAccess(int userId, List<Guid> accountBeDefinitionIds)
        {
            foreach (var a in accountBeDefinitionIds)
            {
                if (DoesUserHaveViewAccess(userId, a))
                    return true;
            }
            return false;
        }
        public bool DoesUserHaveViewAccess(Guid accountBeDefinitionId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return DoesUserHaveAccess(userId, accountBeDefinitionId, (sec) => sec.ViewRequiredPermission);
        }
        public bool DoesUserHaveViewAccess(int userId, Guid accountBeDefinitionId)
        {
            return DoesUserHaveAccess(userId, accountBeDefinitionId, (sec) => sec.ViewRequiredPermission);
        }
        public bool DoesUserHaveAddAccess(Guid accountBeDefinitionId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return DoesUserHaveAccess(userId, accountBeDefinitionId, (sec) => sec.AddRequiredPermission);

        }
        public bool DoesUserHaveEditAccess(Guid accountBeDefinitionId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return DoesUserHaveEditAccess(userId, accountBeDefinitionId);
        }
        public bool DoesUserHaveEditAccess(int userId, Guid accountBeDefinitionId)
        {
            return DoesUserHaveAccess(userId, accountBeDefinitionId, (sec) => sec.EditRequiredPermission);
        }
        public bool DoesUserHaveViewPackageAccess(int userId, Guid accountBeDefinitionId)
        {
            return DoesUserHaveAccess(userId, accountBeDefinitionId, (sec) => sec.ViewPackageRequiredPermission);

        }
        public bool DoesUserHaveAddPackageAccess(int userId, Guid accountBeDefinitionId)
        {
            return DoesUserHaveAccess(userId, accountBeDefinitionId, (sec) => sec.AddPackageRequiredPermission);
        }
        public bool DoesUserHaveEditPackageAccess(int userId, Guid accountBeDefinitionId)
        {
            return DoesUserHaveAccess(userId, accountBeDefinitionId, (sec) => sec.EditPackageRequiredPermission);

        }
        public bool DoesUserHaveViewAccountPackageAccess(int userId, Guid accountBeDefinitionId)
        {
            return  DoesUserHaveAccess(userId, accountBeDefinitionId, (sec) => sec.ViewAccountPackageRequiredPermission);
        }
        public bool DoesUserHaveAddAccountPackageAccess(int userId, Guid accountBeDefinitionId)
        {
            return DoesUserHaveAccess(userId, accountBeDefinitionId, (sec) => sec.AddAccountPackageRequiredPermission);
        }
        public HashSet<Guid> GetLoggedInUserAllowedActionIds(Guid accountBeDefinitionId)
        {
            HashSet<Guid> ids = new HashSet<Guid>();
            var ActionIds = GetAccountActionDefinitions(accountBeDefinitionId);
            IAccountActionDefinitionCheckAccessContext context = new AccountActionDefinitionCheckAccessContext { UserId = SecurityContext.Current.GetLoggedInUserId(), AccountBEDefinitionId = accountBeDefinitionId };
            foreach (var id in ActionIds)
            {
                if (id.ActionDefinitionSettings.DoesUserHaveAccess(context))
                    ids.Add(id.AccountActionDefinitionId);
            }

            return ids;
        }
        public HashSet<Guid> GetLoggedInUserAllowedViewIds(Guid accountBeDefinitionId)
        {
            HashSet<Guid> ids = new HashSet<Guid>();
            var ActionIds = GetAccountViewDefinitions(accountBeDefinitionId);
            IAccountViewDefinitionCheckAccessContext context = new AccountViewDefinitionCheckAccessContext { UserId = SecurityContext.Current.GetLoggedInUserId(), AccountBEDefinitionId = accountBeDefinitionId };
            foreach (var view in ActionIds)
            {
                if (view.Settings.DoesUserHaveAccess(context))
                    ids.Add(view.AccountViewDefinitionId);
            }

            return ids;
        }

        #endregion

        #region Private Methods
        private bool DoesUserHaveAccess(int userId, Guid accountBEDefinitionId, Func<AccountBEDefinitionSecurity, Vanrise.Security.Entities.RequiredPermissionSettings> getRequiredPermissionSetting)
        {
            var accountBEDefinitionSettings = GetAccountBEDefinitionSettings(accountBEDefinitionId);
            if (accountBEDefinitionSettings != null && accountBEDefinitionSettings.Security != null && getRequiredPermissionSetting(accountBEDefinitionSettings.Security) != null)
                return s_securityManager.IsAllowed(getRequiredPermissionSetting(accountBEDefinitionSettings.Security), userId);
            else
                return true;
        }
        private bool IsColumnVisible(Dictionary<string, VRRetailBEVisibilityAccountDefinitionGridColumns> visibleGridColumns, AccountGridColumnDefinition accountGridColumnDefinition)
        {
            if (!visibleGridColumns.ContainsKey(accountGridColumnDefinition.FieldName))
                return false;
            return true;
        }
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

        private bool IsViewVisible(Dictionary<Guid, VRRetailBEVisibilityAccountDefinitionView> visibleViews, AccountViewDefinition accountViewDefinition)
        {
            if (!visibleViews.ContainsKey(accountViewDefinition.AccountViewDefinitionId))
                return false;
            return true;
        }
        private bool IsViewAvailable(AccountViewDefinition accountViewDefinition, Account account)
        {
            if (accountViewDefinition.AvailabilityCondition != null)
                return accountViewDefinition.AvailabilityCondition.Evaluate(new AccountConditionEvaluationContext() { Account = account });
            return true;
        }

        private bool IsActionVisible(Dictionary<Guid, VRRetailBEVisibilityAccountDefinitionAction> visibleViews, AccountActionDefinition accountActionDefinition)
        {
            if (!visibleViews.ContainsKey(accountActionDefinition.AccountActionDefinitionId))
                return false;
            return true;
        }
        private bool IsActionAvailable(AccountActionDefinition accountActionDefinition, Account account)
        {
            if (accountActionDefinition.AvailabilityCondition != null)
                return accountActionDefinition.AvailabilityCondition.Evaluate(new AccountConditionEvaluationContext() { Account = account });
            return true;
        }


        #endregion

        #region Private Mappers
        AccountActionDefinitionInfo AccountActionDefinitionInfoMapper(AccountActionDefinition accountActionDefinition)
        {
            return new AccountActionDefinitionInfo
            {
                AccountActionDefinitionId = accountActionDefinition.AccountActionDefinitionId,
                Name = accountActionDefinition.Name,
            };
        }
        #endregion

    }
}
