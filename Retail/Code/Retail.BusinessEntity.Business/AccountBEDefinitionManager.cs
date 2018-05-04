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
        public string GetAccountBEDefinitionName(Guid accountBEDefinitionId)
        {

            return s_businessEntityDefinitionManager.GetBusinessEntityDefinitionName(accountBEDefinitionId);
        }
        public AccountBEDefinitionSettings GetAccountBEDefinitionSettingsWithHidden(Guid accountBEDefinitionId)
        {
            var businessEntityDefinition = s_businessEntityDefinitionManager.GetBusinessEntityDefinition(accountBEDefinitionId);

            if (businessEntityDefinition == null)
                throw new NullReferenceException(string.Format("businessEntityDefinition Id : {0}", accountBEDefinitionId));

            if (businessEntityDefinition.Settings == null)
                throw new NullReferenceException(string.Format("businessEntityDefinition.Settings Id : {0}", accountBEDefinitionId));

            return businessEntityDefinition.Settings as AccountBEDefinitionSettings;
        }

        public AccountBEDefinitionSettings GetAccountBEDefinitionSettings(Guid accountBEDefinitionId)
        {
            var businessEntityDefinition = s_businessEntityDefinitionManager.GetBusinessEntityDefinition(accountBEDefinitionId);
            businessEntityDefinition.ThrowIfNull("businessEntityDefinition", accountBEDefinitionId);
            return businessEntityDefinition.Settings.CastWithValidate<AccountBEDefinitionSettings>("businessEntityDefinition.Settings", accountBEDefinitionId);
        }
        public List<string> GetAccountBEDefinitionClassifications(Guid accountBEDefinitionId)
        {
            var businessEntityDefinition = GetAccountBEDefinitionSettings(accountBEDefinitionId);
            businessEntityDefinition.ThrowIfNull("businessEntityDefinition", accountBEDefinitionId);
            businessEntityDefinition.Classifications.ThrowIfNull("businessEntityDefinition.Classifications");
            return businessEntityDefinition.Classifications.MapRecords(x => x.Name).ToList();
        }

        public AccountCondition GetPackageAssignmentCondition(Guid accountBEDefinitionId)
        {
            AccountBEDefinitionSettings accountBEDefinitionSettings = this.GetAccountBEDefinitionSettings(accountBEDefinitionId);
            return accountBEDefinitionSettings.PackageAssignmentCondition;
        }
        public AccountGridDefinition GetAccountGridDefinition(Guid accountBEDefinitionId)
        {
            AccountBEDefinitionSettings accountBEDefinitionSettings = this.GetAccountBEDefinitionSettings(accountBEDefinitionId);

            AccountGridDefinition accountGridDefinition = accountBEDefinitionSettings.GridDefinition;
            if (accountGridDefinition == null)
                throw new NullReferenceException(string.Format("accountGridDefinition for BusinessEntityDefinition Id : {0}", accountBEDefinitionId));

            return accountGridDefinition;
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
        public bool IsFinancialAccountModuleUsed(Guid accountBEDefinitionId)
        {
            var accountBEDefinitionSettings = GetAccountBEDefinitionSettings(accountBEDefinitionId);
            return accountBEDefinitionSettings.UseFinancialAccountModule;
        }
        public List<AccountViewDefinition> GetAccountViewDefinitions(Guid accountBEDefinitionId)
        {
            AccountBEDefinitionSettings accountBEDefinitionSettings = this.GetAccountBEDefinitionSettings(accountBEDefinitionId);

            List<AccountViewDefinition> accountViewDefinitions = accountBEDefinitionSettings.AccountViewDefinitions;
            if (accountViewDefinitions == null)
                throw new NullReferenceException(string.Format("accountViewDefinitions for BusinessEntityDefinition Id : {0}", accountBEDefinitionId));

            return accountViewDefinitions;
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
        public T GetAccountViewDefinitionSettings<T>(Guid accountBEDefinitionId, Guid accountViewDefinitionId) where T: AccountViewDefinitionSettings
        {
            List<AccountViewDefinition> accountViewDefinitions = GetAccountViewDefinitions(accountBEDefinitionId);

            AccountViewDefinition accountViewDefinition = accountViewDefinitions.FindRecord(itm => itm.AccountViewDefinitionId == accountViewDefinitionId);
            if (accountViewDefinition == null)
                throw new NullReferenceException(string.Format("AccountViewDefinition of AccountViewDefinitionId: {0} and BusinessEntityDefinitionId: {1}", accountViewDefinitionId, accountBEDefinitionId));

            if (accountViewDefinition.Settings == null)
                throw new NullReferenceException(string.Format("AccountViewDefinition.Settings of AccountViewDefinitionId: {0} and BusinessEntityDefinitionId: {1}", accountViewDefinitionId, accountBEDefinitionId));

            T accountViewDefinitionSettings = accountViewDefinition.Settings as T;
            if (accountViewDefinitionSettings == null)
                throw new Exception(String.Format("accountViewDefinitionSettings should be of type '{0}'. it is of type '{1}'", typeof(T), accountViewDefinitionSettings.GetType()));

            return accountViewDefinitionSettings;
        }

        public List<AccountActionDefinition> GetAccountActionDefinitions(Guid accountBEDefinitionId)
        {
            AccountBEDefinitionSettings accountBEDefinitionSettings = this.GetAccountBEDefinitionSettings(accountBEDefinitionId);

            List<AccountActionDefinition> accountActionDefinitions = accountBEDefinitionSettings.ActionDefinitions;
            if (accountActionDefinitions == null)
                throw new NullReferenceException(string.Format("accountActionDefinitions for BusinessEntityDefinition Id : {0}", accountBEDefinitionId));

            return accountActionDefinitions;
        }
        public AccountActionDefinition GetAccountActionDefinition(Guid accountBEDefinitionId, Guid actionDefinitionId)
        {
            var accountActionDefinitions = GetAccountActionDefinitions(accountBEDefinitionId);
            if (accountActionDefinitions == null)
                throw new NullReferenceException(String.Format("accountActionDefinitions AccountBEDefinitionId '{0}'", accountBEDefinitionId));

            return accountActionDefinitions.FirstOrDefault(x => x.AccountActionDefinitionId == actionDefinitionId);
        }
        public Guid GetAccountBEStatusDefinitionId(Guid accountBEDefinitionId)
        {
            var accountBEDefinitionSettings = GetAccountBEDefinitionSettings(accountBEDefinitionId);
            accountBEDefinitionSettings.ThrowIfNull("accountBEDefinitionSettings", accountBEDefinitionId);
            return accountBEDefinitionSettings.StatusBEDefinitionId;
        }
        public AccountActionDefinitionSettings GetAccountActionDefinitionSettings(Guid accountBEDefinitionId, Guid actionDefinitionId)
        {
            var accountActionDefinition = GetAccountActionDefinition(accountBEDefinitionId, actionDefinitionId);
            if (accountActionDefinition == null)
                throw new NullReferenceException(string.Format("accountActionDefinition of accountBEDefinitionId {0} nad actionDefinitionId {1}", accountBEDefinitionId, actionDefinitionId));
            return accountActionDefinition.ActionDefinitionSettings;
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
        public bool CheckUseRemoteSelector(Guid accountBEDefinitionId)
        {
            var accountBEDefinitionSettings = GetAccountBEDefinitionSettings(accountBEDefinitionId);
            return accountBEDefinitionSettings.UseRemoteSelector;
        }
        public List<AccountExtraFieldDefinition> GetAccountExtraFieldDefinitions(Guid accountBEDefinitionId)
        {
            AccountBEDefinitionSettings accountBEDefinitionSettings = this.GetAccountBEDefinitionSettings(accountBEDefinitionId);

            return accountBEDefinitionSettings.AccountExtraFieldDefinitions;
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
        public IEnumerable<FinancialAccountLocatorConfig> GetFinancialAccountLocatorConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<FinancialAccountLocatorConfig>(FinancialAccountLocatorConfig.EXTENSION_TYPE);
        }
        public IEnumerable<AccountExtraFieldDefinitionConfig> GetAccountExtraFieldDefinitionSettingsConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<AccountExtraFieldDefinitionConfig>(AccountExtraFieldDefinitionConfig.EXTENSION_TYPE);
        }

        public IEnumerable<AccountBulkActionSettingsConfig> GetAccountBulkActionSettingsConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<AccountBulkActionSettingsConfig>(AccountBulkActionSettingsConfig.EXTENSION_TYPE);
        }

        public List<AccountBEClassification> GetAccountBEDefinitionClassificationsInfo(Guid accountBEDefinitionId)
        {
            var accountBEDefinitionSettings = GetAccountBEDefinitionSettings(accountBEDefinitionId);
            accountBEDefinitionSettings.ThrowIfNull("accountBEDefinitionSettings", accountBEDefinitionId);
            return accountBEDefinitionSettings.Classifications;
        }

        #endregion

        #region Private Methods

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
                AccountBEManager accountBEManager = new Business.AccountBEManager();
                return accountBEManager.EvaluateAccountCondition(accountBEDefinitionId, parentAccountId.Value, gridColumnDefinition.SubAccountsAvailabilityCondition);
            }

            return true;
        }


        public List<AccountActionDefinition>  GetAllAccountActionDefinitions()
        {

            return s_businessEntityDefinitionManager.GetCachedOrCreate("AccountBEDefinitionManager_GetAllAccountActionDefinitions", () =>
            {
                var accountBeDefinitions = s_businessEntityDefinitionManager.GetCachedBusinessEntityDefinitions().Where(x => x.Value.Settings.ConfigId == AccountBEDefinitionSettings.s_configId);

                List<AccountActionDefinition> accountActionDefinitions = new List<AccountActionDefinition>();
                foreach (var def in accountBeDefinitions)
                {
                    var accountBeActions = GetAccountActionDefinitions(def.Key);
                    accountActionDefinitions.AddRange(accountBeActions);
                }
                return accountActionDefinitions;
            });
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
            {
                AccountBEManager accountBEManager = new AccountBEManager();
                return accountBEManager.EvaluateAccountCondition(account, accountViewDefinition.AvailabilityCondition);
            }
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
            {
                AccountBEManager accountBEManager = new AccountBEManager();
                return accountBEManager.EvaluateAccountCondition(account, accountActionDefinition.AvailabilityCondition);
            }
            return true;
        }

        private bool DoesUserHaveAccess(int userId, Guid accountBEDefinitionId, Func<AccountBEDefinitionSecurity, Vanrise.Security.Entities.RequiredPermissionSettings> getRequiredPermissionSetting)
        {
            var accountBEDefinitionSettings = GetAccountBEDefinitionSettings(accountBEDefinitionId);
            if (accountBEDefinitionSettings != null && accountBEDefinitionSettings.Security != null && getRequiredPermissionSetting(accountBEDefinitionSettings.Security) != null)
                return s_securityManager.IsAllowed(getRequiredPermissionSetting(accountBEDefinitionSettings.Security), userId);
            else
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
            return DoesUserHaveAccess(userId, accountBeDefinitionId, (sec) => sec.ViewAccountPackageRequiredPermission);
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
        public bool DoesUserHaveViewProductAccess(int userId, Guid accountBeDefinitionId)
        {
            return DoesUserHaveAccess(userId, accountBeDefinitionId, (sec) => sec.ViewProductRequiredPermission);

        }
        public bool DoesUserHaveAddProductAccess(int userId, Guid accountBeDefinitionId)
        {
            return DoesUserHaveAccess(userId, accountBeDefinitionId, (sec) => sec.AddProductRequiredPermission);
        }
        public bool DoesUserHaveEditProductAccess(int userId, Guid accountBeDefinitionId)
        {
            return DoesUserHaveAccess(userId, accountBeDefinitionId, (sec) => sec.EditProductRequiredPermission);

        }
        public bool DoesUserHaveViewsAccess(int userId, Guid accountBEDefinitionId)
        {
            var viewDefinitions = GetAccountViewDefinitions(accountBEDefinitionId);
            var viewcontext = new AccountViewDefinitionCheckAccessContext
            {
                UserId = userId,
                AccountBEDefinitionId = accountBEDefinitionId
            };
            foreach (var v in viewDefinitions)
            {
                if (v != null && v.Settings != null && v.Settings.DoesUserHaveAccess(viewcontext) == true)
                    return true;
            }
            return false;
        }
        #endregion
    }
}
