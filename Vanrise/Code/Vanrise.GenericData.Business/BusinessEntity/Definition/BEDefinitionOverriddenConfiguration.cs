using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace Vanrise.GenericData.Business
{
    public class BEDefinitionOverriddenConfiguration : OverriddenConfigurationExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("8DC29F02-7197-4C60-8E21-CBDE0C2AE87B"); }
        }

        public Guid BusinessEntityDefinitionId { get; set; }

        public string OverriddenTitle { get; set; }

        public BusinessEntityDefinitionSettings OverriddenSettings { get; set; }

        public override Type GetBehaviorType(IOverriddenConfigurationGetBehaviorContext context)
        {
            return typeof(BEDefinitionOverriddenConfigurationBehavior);
        }
    }

    public class BEDefinitionOverriddenConfigurationBehavior : OverriddenConfigurationBehavior
    {
        public override void GenerateScript(IOverriddenConfigurationBehaviorGenerateScriptContext context)
        {
            StringBuilder scriptBuilder = new StringBuilder();
            BusinessEntityDefinitionManager businessEntityDefinitionManager = new BusinessEntityDefinitionManager();
            List<VRLoggableEntityBase> loggableEntities = new List<VRLoggableEntityBase>();
            foreach (var config in context.Configs)
            {
                BEDefinitionOverriddenConfiguration beDefinitionConfig = config.Settings.ExtendedSettings.CastWithValidate<BEDefinitionOverriddenConfiguration>("beDefinitionConfig", config.OverriddenConfigurationId);

                var beDefinition = businessEntityDefinitionManager.GetBusinessEntityDefinition(beDefinitionConfig.BusinessEntityDefinitionId);
                beDefinition.ThrowIfNull("beDefinition", beDefinitionConfig.BusinessEntityDefinitionId);
                beDefinition = beDefinition.VRDeepCopy();
                if (!String.IsNullOrEmpty(beDefinitionConfig.OverriddenTitle))
                    beDefinition.Title = beDefinitionConfig.OverriddenTitle;
                if (beDefinitionConfig.OverriddenSettings != null)
                    beDefinition.Settings = beDefinitionConfig.OverriddenSettings;
                if (scriptBuilder.Length > 0)
                {
                    scriptBuilder.Append(",");
                    scriptBuilder.AppendLine();
                }
                scriptBuilder.AppendFormat(@"('{0}','{1}','{2}','{3}')", beDefinition.BusinessEntityDefinitionId, beDefinition.Name, beDefinition.Title, Serializer.Serialize(beDefinition.Settings));
                if (beDefinition.Settings != null)
                {
                    var loggableEntity = beDefinition.Settings.GetLoggableEntity(new BusinessEntityDefinitionSettingsGetLoggableEntityContext { BEDefinition = beDefinition });
                    if (loggableEntity != null)
                        loggableEntities.Add(loggableEntity);
                }
            }
            string script = String.Format(@"set nocount on;
;with cte_data([ID],[Name],[Title],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
{0}
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Settings]))
merge	[genericdata].[BusinessEntityDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Title],[Settings])
	values(s.[ID],s.[Name],s.[Title],s.[Settings]);", scriptBuilder);
            context.AddEntityScript("[genericdata].[BusinessEntityDefinition]", script);
            if(loggableEntities.Count> 0)
            {
                string loggableEntityScriptEntityName;
                var loggableEntitiesScript = new Vanrise.Common.Business.VRLoggableEntityManager().GenerateLoggableEntitiesScript(loggableEntities, out loggableEntityScriptEntityName);
                context.AddEntityScript(String.Format("{0} - Business Entity Definitions", loggableEntityScriptEntityName), loggableEntitiesScript);
            }
        }

        #region Private Classes

        public class BusinessEntityDefinitionSettingsGetLoggableEntityContext : IBusinessEntityDefinitionSettingsGetLoggableEntityContext
        {
            public BusinessEntityDefinition BEDefinition
            {
                get;
                set;
            }
        }


        #endregion
    }

}
