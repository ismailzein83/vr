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
    public class AccountPartDefinitionOverriddenConfiguration : OverriddenConfigurationExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("C01FA066-28C8-4225-9F59-39F5EECF86ED"); }
        }

        public Guid AccountPartDefinitionId { get; set; }

        public string OverriddenTitle { get; set; }

        public AccountPartDefinitionSettings OverriddenSettings { get; set; }

        public override Type GetBehaviorType(IOverriddenConfigurationGetBehaviorContext context)
        {
            return typeof(AccountTypeOverriddenConfigurationBehavior);
        }

        #region Private Classes

        private class AccountTypeOverriddenConfigurationBehavior : OverriddenConfigurationBehavior
        {
            public override void GenerateScript(IOverriddenConfigurationBehaviorGenerateScriptContext context)
            {
                AccountPartDefinitionManager accountPartManager = new AccountPartDefinitionManager();
                List<AccountPartDefinition> accountParts = new List<AccountPartDefinition>();
                foreach (var config in context.Configs)
                {
                    AccountPartDefinitionOverriddenConfiguration accountPartConfig = config.Settings.ExtendedSettings.CastWithValidate<AccountPartDefinitionOverriddenConfiguration>("accountTypeConfig", config.OverriddenConfigurationId);

                    var accountPart = accountPartManager.GetAccountPartDefinition(accountPartConfig.AccountPartDefinitionId);
                    accountPart.ThrowIfNull("accountPart", accountPartConfig.AccountPartDefinitionId);
                    accountPart = accountPart.VRDeepCopy();
                    if (!String.IsNullOrEmpty(accountPartConfig.OverriddenTitle))
                    {
                        accountPart.Name = accountPartConfig.OverriddenTitle;
                        accountPart.Title = accountPartConfig.OverriddenTitle;
                    }
                    if (accountPartConfig.OverriddenSettings != null)
                        accountPart.Settings = accountPartConfig.OverriddenSettings;
                    accountParts.Add(accountPart);                    
                }
                GenerateScript(accountParts, context.AddEntityScript);
            }

            public override void GenerateDevScript(IOverriddenConfigurationBehaviorGenerateDevScriptContext context)
            {
                IEnumerable<Guid> ids = context.Configs.Select(config => config.Settings.ExtendedSettings.CastWithValidate<AccountPartDefinitionOverriddenConfiguration>("config.Settings.ExtendedSettings", config.OverriddenConfigurationId).AccountPartDefinitionId).Distinct();
                AccountPartDefinitionManager accountPartManager = new AccountPartDefinitionManager();
                List<AccountPartDefinition> accountParts = new List<AccountPartDefinition>();
                foreach (var id in ids)
                {
                    var accountPart = accountPartManager.GetAccountPartDefinition(id);
                    accountPart.ThrowIfNull("accountPart", id);                   
                    accountParts.Add(accountPart);                    
                }
                GenerateScript(accountParts, context.AddEntityScript);
            }

            private void GenerateScript(List<AccountPartDefinition> accountParts, Action<string, string> addEntityScript)
            {
                IAccountPartDefinitionDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountPartDefinitionDataManager>();
                dataManager.GenerateScript(accountParts, addEntityScript);
            }
        }

        #endregion
    }
}
