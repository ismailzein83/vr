using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRObjectTypeDefinitionOverriddenConfiguration : OverriddenConfigurationExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("E1CE89E4-178E-4BB9-9AF2-0163B2712C75"); }
        }

        public Guid VRObjectTypeDefinitionId { get; set; }

        public string OverriddenName { get; set; }

        public VRObjectTypeDefinitionSettings OverriddenSettings { get; set; }

        public override Type GetBehaviorType(IOverriddenConfigurationGetBehaviorContext context)
        {
            return typeof(VRObjectTypeDefinitionOverriddenConfigurationBehavior);
        }

        #region Private Methods

        private class VRObjectTypeDefinitionOverriddenConfigurationBehavior : OverriddenConfigurationBehavior
        {
            public override void GenerateScript(IOverriddenConfigurationBehaviorGenerateScriptContext context)
            {
                StringBuilder scriptBuilder = new StringBuilder();
                VRObjectTypeDefinitionManager objTypeDefManager = new VRObjectTypeDefinitionManager();
                foreach (var config in context.Configs)
                {
                    VRObjectTypeDefinitionOverriddenConfiguration objTypeDefConfig = config.Settings.ExtendedSettings.CastWithValidate<VRObjectTypeDefinitionOverriddenConfiguration>("objTypeDefConfig", config.OverriddenConfigurationId);

                    var objTypeDef = objTypeDefManager.GetVRObjectTypeDefinition(objTypeDefConfig.VRObjectTypeDefinitionId);
                    objTypeDef.ThrowIfNull("objTypeDef", objTypeDefConfig.VRObjectTypeDefinitionId);
                    objTypeDef.Settings.ThrowIfNull("objTypeDef.Settings", objTypeDefConfig.VRObjectTypeDefinitionId);
                    objTypeDef = objTypeDef.VRDeepCopy();
                    if (!String.IsNullOrEmpty(objTypeDefConfig.OverriddenName))
                        objTypeDef.Name = objTypeDefConfig.OverriddenName;
                    if (objTypeDefConfig.OverriddenSettings != null)
                        objTypeDef.Settings = objTypeDefConfig.OverriddenSettings;
                    if (scriptBuilder.Length > 0)
                    {
                        scriptBuilder.Append(",");
                        scriptBuilder.AppendLine();
                    }
                    scriptBuilder.AppendFormat(@"('{0}','{1}','{2}')", objTypeDef.VRObjectTypeDefinitionId, objTypeDef.Name, Serializer.Serialize(objTypeDef.Settings));
                }
                string script = String.Format(@"set nocount on;
;with cte_data([ID],[Name],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
{0}
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Settings]))
merge	[common].[VRObjectTypeDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Settings])
	values(s.[ID],s.[Name],s.[Settings]);", scriptBuilder);
                context.AddEntityScript("[common].[VRObjectTypeDefinition]", script);
            }
        }

        #endregion
    }
}
