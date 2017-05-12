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
    public class ServiceTypeOverriddenConfiguration : OverriddenConfigurationExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("4F2A2B2F-CAA6-423A-A08F-39DE8587E3BA"); }
        }

        public Guid ServiceTypeId { get; set; }

        public string OverriddenTitle { get; set; }

        public ServiceTypeSettings OverriddenSettings { get; set; }

        public override Type GetBehaviorType(IOverriddenConfigurationGetBehaviorContext context)
        {
            return typeof(ServiceTypeOverriddenConfigurationBehavior);
        }

        #region Private Classes

        public class ServiceTypeOverriddenConfigurationBehavior : OverriddenConfigurationBehavior
        {
            public override void GenerateScript(IOverriddenConfigurationBehaviorGenerateScriptContext context)
            {
                StringBuilder scriptBuilder = new StringBuilder();
                ServiceTypeManager serviceTypeManager = new ServiceTypeManager();
                foreach (var config in context.Configs)
                {
                    ServiceTypeOverriddenConfiguration serviceTypeConfig = config.Settings.ExtendedSettings.CastWithValidate<ServiceTypeOverriddenConfiguration>("serviceTypeConfig", config.OverriddenConfigurationId);

                    var serviceType = serviceTypeManager.GetServiceType(serviceTypeConfig.ServiceTypeId);
                    serviceType.ThrowIfNull("serviceType", serviceTypeConfig.ServiceTypeId);
                    serviceType = serviceType.VRDeepCopy();
                    if (!String.IsNullOrEmpty(serviceTypeConfig.OverriddenTitle))
                    {
                        serviceType.Name = serviceTypeConfig.OverriddenTitle.Replace(" ", "");
                        serviceType.Title = serviceTypeConfig.OverriddenTitle;
                    }
                    if (serviceTypeConfig.OverriddenSettings != null)
                        serviceType.Settings = serviceTypeConfig.OverriddenSettings;
                    if (scriptBuilder.Length > 0)
                    {
                        scriptBuilder.Append(",");
                        scriptBuilder.AppendLine();
                    }
                    scriptBuilder.AppendFormat(@"('{0}','{1}','{2}','{3}','{4}')", serviceType.ServiceTypeId, serviceType.Name, serviceType.Title, serviceType.AccountBEDefinitionId, Serializer.Serialize(serviceType.Settings));
                }
                string script = String.Format(@"set nocount on;
;with cte_data([ID],[Name],[Title],[AccountBEDefinitionId],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
{0}
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[AccountBEDefinitionId],[Settings]))
merge	[Retail].[ServiceType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[AccountBEDefinitionId] = s.[AccountBEDefinitionId],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Title],[AccountBEDefinitionId],[Settings])
	values(s.[ID],s.[Name],s.[Title],s.[AccountBEDefinitionId],s.[Settings]);", scriptBuilder);
                context.AddEntityScript("[Retail_BE].[ServiceType]", script);
            }
        }

        #endregion
    }
}
