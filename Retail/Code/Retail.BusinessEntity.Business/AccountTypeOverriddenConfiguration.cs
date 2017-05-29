using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
using Retail.BusinessEntity.Data;

namespace Retail.BusinessEntity.Business
{
    public class AccountTypeOverriddenConfiguration : OverriddenConfigurationExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("22C9E36D-D328-4220-83E8-E45AD1B005D8"); }
        }

        public Guid AccountTypeId { get; set; }

        public string OverriddenTitle { get; set; }

        public AccountTypeSettings OverriddenSettings { get; set; }

        public override Type GetBehaviorType(IOverriddenConfigurationGetBehaviorContext context)
        {
            return typeof(AccountTypeOverriddenConfigurationBehavior);
        }

        #region Private Classes

        public class AccountTypeOverriddenConfigurationBehavior : OverriddenConfigurationBehavior
        {
            public override void GenerateScript(IOverriddenConfigurationBehaviorGenerateScriptContext context)
            {
                AccountTypeManager accountTypeManager = new AccountTypeManager();
                List<AccountType> accountTypes = new List<AccountType>();
                foreach (var config in context.Configs)
                {
                    AccountTypeOverriddenConfiguration accountTypeConfig = config.Settings.ExtendedSettings.CastWithValidate<AccountTypeOverriddenConfiguration>("accountTypeConfig", config.OverriddenConfigurationId);

                    var accountType = accountTypeManager.GetAccountType(accountTypeConfig.AccountTypeId);
                    accountType.ThrowIfNull("accountType", accountTypeConfig.AccountTypeId);
                    accountType = accountType.VRDeepCopy();
                    if (!String.IsNullOrEmpty(accountTypeConfig.OverriddenTitle))
                    {
                        accountType.Name = accountTypeConfig.OverriddenTitle;
                        accountType.Title = accountTypeConfig.OverriddenTitle;
                    }
                    if (accountTypeConfig.OverriddenSettings != null)
                        accountType.Settings = accountTypeConfig.OverriddenSettings;
                    accountTypes.Add(accountType);                    
                }
                GenerateScript(accountTypes, context.AddEntityScript);
            }

            public override void GenerateDevScript(IOverriddenConfigurationBehaviorGenerateDevScriptContext context)
            {
                IEnumerable<Guid> ids = context.Configs.Select(config => config.Settings.ExtendedSettings.CastWithValidate<AccountTypeOverriddenConfiguration>("config.Settings.ExtendedSettings", config.OverriddenConfigurationId).AccountTypeId).Distinct();
                AccountTypeManager accountTypeManager = new AccountTypeManager();
                List<AccountType> accountTypes = new List<AccountType>();
                foreach (var id in ids)
                {
                    var accountType = accountTypeManager.GetAccountType(id);
                    accountType.ThrowIfNull("accountType", id);
                    accountTypes.Add(accountType);
                }
                GenerateScript(accountTypes, context.AddEntityScript);
            }

            private void GenerateScript(List<AccountType> accountTypes, Action<string, string> addEntityScript)
            {
                IAccountTypeDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountTypeDataManager>();
                dataManager.GenerateScript(accountTypes, addEntityScript);
            }
        }

        #endregion
    }
}
