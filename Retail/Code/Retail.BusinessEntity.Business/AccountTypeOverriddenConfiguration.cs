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
                StringBuilder scriptBuilder = new StringBuilder();
                AccountTypeManager accountTypeManager = new AccountTypeManager();
                foreach (var config in context.Configs)
                {
                    AccountTypeOverriddenConfiguration accountTypeConfig = config.Settings.ExtendedSettings.CastWithValidate<AccountTypeOverriddenConfiguration>("accountTypeConfig", config.OverriddenConfigurationId);

                    var accountType = accountTypeManager.GetAccountType(accountTypeConfig.AccountTypeId);
                    accountType.ThrowIfNull("accountType", accountTypeConfig.AccountTypeId);
                    accountType = accountType.VRDeepCopy();
                    if (!String.IsNullOrEmpty(accountTypeConfig.OverriddenTitle))
                        accountType.Title = accountTypeConfig.OverriddenTitle;
                    if (accountTypeConfig.OverriddenSettings != null)
                        accountType.Settings = accountTypeConfig.OverriddenSettings;
                    if (scriptBuilder.Length > 0)
                    {
                        scriptBuilder.Append(",");
                        scriptBuilder.AppendLine();
                    }
                    scriptBuilder.AppendFormat(@"('{0}','{1}','{2}','{3}','{4}')", accountType.AccountTypeId, accountType.Name, accountType.Title, accountType.AccountBEDefinitionId, Serializer.Serialize(accountType.Settings));
                }
                string script = String.Format(@"set nocount on;
;with cte_data([ID],[Name],[Title],[AccountBEDefinitionID],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
{0}
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[AccountBEDefinitionID],[Settings]))
merge	[Retail_BE].[AccountType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[AccountBEDefinitionID] = s.[AccountBEDefinitionID],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Title],[AccountBEDefinitionID],[Settings])
	values(s.[ID],s.[Name],s.[Title],s.[AccountBEDefinitionID],s.[Settings]);", scriptBuilder);
                context.AddEntityScript("[Retail_BE].[AccountType]", script);
            }
        }

        #endregion
    }
}
