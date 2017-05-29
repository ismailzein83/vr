using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
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
                VRObjectTypeDefinitionManager objTypeDefManager = new VRObjectTypeDefinitionManager();
                List<VRObjectTypeDefinition> objTypeDefs = new List<VRObjectTypeDefinition>();
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
                    objTypeDefs.Add(objTypeDef);
                    
                }
                GenerateScript(objTypeDefs, context.AddEntityScript);
            }

            public override void GenerateDevScript(IOverriddenConfigurationBehaviorGenerateDevScriptContext context)
            {
                IEnumerable<Guid> ids = context.Configs.Select(config => config.Settings.ExtendedSettings.CastWithValidate<VRObjectTypeDefinitionOverriddenConfiguration>("config.Settings.ExtendedSettings", config.OverriddenConfigurationId).VRObjectTypeDefinitionId).Distinct();
                VRObjectTypeDefinitionManager objTypeDefManager = new VRObjectTypeDefinitionManager();
                List<VRObjectTypeDefinition> objTypeDefs = new List<VRObjectTypeDefinition>();
                foreach (var id in ids)
                {
                    var objTypeDef = objTypeDefManager.GetVRObjectTypeDefinition(id);
                    objTypeDef.ThrowIfNull("objTypeDef", id);
                    objTypeDefs.Add(objTypeDef);
                }
                GenerateScript(objTypeDefs, context.AddEntityScript);
            }

            private void GenerateScript(List<VRObjectTypeDefinition> objTypeDefs, Action<string, string> addEntityScript)
            {
                IVRObjectTypeDefinitionDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRObjectTypeDefinitionDataManager>();
                dataManager.GenerateScript(objTypeDefs, addEntityScript);
            }
        }

        #endregion
    }
}
