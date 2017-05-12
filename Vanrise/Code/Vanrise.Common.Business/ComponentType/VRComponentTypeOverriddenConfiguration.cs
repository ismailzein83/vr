using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRComponentTypeOverriddenConfiguration : OverriddenConfigurationExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("6C7BA6EC-1FC6-45E7-BF8D-5F8074DF98E0"); }
        }

        public Guid VRComponentTypeId { get; set; }

        public string OverriddenName { get; set; }

        public VRComponentTypeSettings OverriddenSettings { get; set; }

        public override Type GetBehaviorType(IOverriddenConfigurationGetBehaviorContext context)
        {
            return typeof(VRComponentTypeOverriddenConfigurationBehavior);
        }

        #region Private Methods
        
        private class VRComponentTypeOverriddenConfigurationBehavior : OverriddenConfigurationBehavior
        {
            public override void GenerateScript(IOverriddenConfigurationBehaviorGenerateScriptContext context)
            {
                StringBuilder scriptBuilder = new StringBuilder();
                VRComponentTypeManager componentTypeManager = new VRComponentTypeManager();
                foreach (var config in context.Configs)
                {
                    VRComponentTypeOverriddenConfiguration componentTypeConfig = config.Settings.ExtendedSettings.CastWithValidate<VRComponentTypeOverriddenConfiguration>("componentTypeConfig", config.OverriddenConfigurationId);

                    var componentType = componentTypeManager.GetComponentType(componentTypeConfig.VRComponentTypeId);
                    componentType.ThrowIfNull("componentType", componentTypeConfig.VRComponentTypeId);
                    componentType.Settings.ThrowIfNull("componentType.Settings", componentTypeConfig.VRComponentTypeId);
                    componentType = componentType.VRDeepCopy();
                    if (!String.IsNullOrEmpty(componentTypeConfig.OverriddenName))
                        componentType.Name = componentTypeConfig.OverriddenName;
                    if (componentTypeConfig.OverriddenSettings != null)
                        componentType.Settings = componentTypeConfig.OverriddenSettings;
                    if (scriptBuilder.Length > 0)
                    {
                        scriptBuilder.Append(",");
                        scriptBuilder.AppendLine();
                    }
                    scriptBuilder.AppendFormat(@"('{0}','{1}','{2}','{3}')", componentType.VRComponentTypeId, componentType.Name, componentType.Settings.VRComponentTypeConfigId, Serializer.Serialize(componentType.Settings));
                }
                string script = String.Format(@"set nocount on;
;with cte_data([ID],[Name],[ConfigID],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
{0}
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[ConfigID],[Settings]))
merge	[common].[VRComponentType] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[ConfigID] = s.[ConfigID],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[ConfigID],[Settings])
	values(s.[ID],s.[Name],s.[ConfigID],s.[Settings]);", scriptBuilder);
                context.AddEntityScript("[common].[VRComponentType]", script);
            }
        }

        #endregion
    }
}
