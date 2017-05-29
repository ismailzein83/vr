using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
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
                VRComponentTypeManager componentTypeManager = new VRComponentTypeManager();
                List<VRComponentType> componentTypes = new List<VRComponentType>();
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
                    componentTypes.Add(componentType);
                }
                GenerateScript(componentTypes, context.AddEntityScript);
            }

            public override void GenerateDevScript(IOverriddenConfigurationBehaviorGenerateDevScriptContext context)
            {
                IEnumerable<Guid> ids = context.Configs.Select(config => config.Settings.ExtendedSettings.CastWithValidate<VRComponentTypeOverriddenConfiguration>("config.Settings.ExtendedSettings", config.OverriddenConfigurationId).VRComponentTypeId).Distinct();
                VRComponentTypeManager componentTypeManager = new VRComponentTypeManager();
                List<VRComponentType> componentTypes = new List<VRComponentType>();
                foreach (var id in ids)
                {
                    var componentType = componentTypeManager.GetComponentType(id);
                    componentType.ThrowIfNull("componentType", id);
                    componentTypes.Add(componentType);
                }
                GenerateScript(componentTypes, context.AddEntityScript);
            }

            private void GenerateScript(List<VRComponentType> componentTypes, Action<string, string> addEntityScript)
            {
                IVRComponentTypeDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRComponentTypeDataManager>();
                dataManager.GenerateScript(componentTypes, addEntityScript);
            }
        }

        #endregion
    }
}
