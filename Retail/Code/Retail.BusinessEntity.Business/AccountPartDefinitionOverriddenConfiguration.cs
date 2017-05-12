using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;

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
                StringBuilder scriptBuilder = new StringBuilder();
                AccountPartDefinitionManager accountPartManager = new AccountPartDefinitionManager();
                foreach (var config in context.Configs)
                {
                    AccountPartDefinitionOverriddenConfiguration accountPartConfig = config.Settings.ExtendedSettings.CastWithValidate<AccountPartDefinitionOverriddenConfiguration>("accountTypeConfig", config.OverriddenConfigurationId);

                    var accountPart = accountPartManager.GetAccountPartDefinition(accountPartConfig.AccountPartDefinitionId);
                    accountPart.ThrowIfNull("accountPart", accountPartConfig.AccountPartDefinitionId);
                    accountPart = accountPart.VRDeepCopy();
                    if (!String.IsNullOrEmpty(accountPartConfig.OverriddenTitle))
                        accountPart.Title = accountPartConfig.OverriddenTitle;
                    if (accountPartConfig.OverriddenSettings != null)
                        accountPart.Settings = accountPartConfig.OverriddenSettings;
                    if (scriptBuilder.Length > 0)
                    {
                        scriptBuilder.Append(",");
                        scriptBuilder.AppendLine();
                    }
                    scriptBuilder.AppendFormat(@"('{0}','{1}','{2}','{3}')", accountPart.AccountPartDefinitionId, accountPart.Title, accountPart.Name, Serializer.Serialize(accountPart.Settings));
                }
                string script = String.Format(@"set nocount on;
;with cte_data([ID],[Title],[Name],[Details])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
{0}
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Title],[Name],[Details]))
merge	[Retail_BE].[AccountPartDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Title] = s.[Title],[Name] = s.[Name],[Details] = s.[Details]
when not matched by target then
	insert([ID],[Title],[Name],[Details])
	values(s.[ID],s.[Title],s.[Name],s.[Details]);", scriptBuilder);
                context.AddEntityScript("[Retail_BE].[AccountPartDefinition]", script);
            }
        }

        #endregion
    }
}
