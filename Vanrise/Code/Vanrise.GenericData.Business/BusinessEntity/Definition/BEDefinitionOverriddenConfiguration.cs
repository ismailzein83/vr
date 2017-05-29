using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Data;

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
            BusinessEntityDefinitionManager businessEntityDefinitionManager = new BusinessEntityDefinitionManager();
            List<BusinessEntityDefinition> beDefinitions = new List<BusinessEntityDefinition>();
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
                beDefinitions.Add(beDefinition);
                
            }
            GenerateScript(beDefinitions, context.AddEntityScript);
        }

        public override void GenerateDevScript(IOverriddenConfigurationBehaviorGenerateDevScriptContext context)
        {
            IEnumerable<Guid> ids = context.Configs.Select(config => config.Settings.ExtendedSettings.CastWithValidate<BEDefinitionOverriddenConfiguration>("config.Settings.ExtendedSettings", config.OverriddenConfigurationId).BusinessEntityDefinitionId).Distinct();
            BusinessEntityDefinitionManager businessEntityDefinitionManager = new BusinessEntityDefinitionManager();
            List<BusinessEntityDefinition> beDefinitions = new List<BusinessEntityDefinition>();
            foreach (var id in ids)
            {
                var beDefinition = businessEntityDefinitionManager.GetBusinessEntityDefinition(id);
                beDefinition.ThrowIfNull("beDefinition", id);
                beDefinitions.Add(beDefinition);
            }
            GenerateScript(beDefinitions, context.AddEntityScript);
        } 

        private void GenerateScript(List<BusinessEntityDefinition> beDefinitions, Action<string, string> addEntityScript)
        {
            IBusinessEntityDefinitionDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IBusinessEntityDefinitionDataManager>();
            dataManager.GenerateScript(beDefinitions, addEntityScript);
            List<VRLoggableEntityBase> loggableEntities = new List<VRLoggableEntityBase>();
            foreach (var beDefinition in beDefinitions)
            {
                if (beDefinition.Settings != null)
                {
                    var loggableEntity = beDefinition.Settings.GetLoggableEntity(new BusinessEntityDefinitionSettingsGetLoggableEntityContext { BEDefinition = beDefinition });
                    if (loggableEntity != null)
                        loggableEntities.Add(loggableEntity);
                }
            }
            if (loggableEntities.Count > 0)
            {
                string loggableEntityScriptEntityName;
                var loggableEntitiesScript = new Vanrise.Common.Business.VRLoggableEntityManager().GenerateLoggableEntitiesScript(loggableEntities, out loggableEntityScriptEntityName);
                addEntityScript(String.Format("{0} - Business Entity Definitions", loggableEntityScriptEntityName), loggableEntitiesScript);
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
