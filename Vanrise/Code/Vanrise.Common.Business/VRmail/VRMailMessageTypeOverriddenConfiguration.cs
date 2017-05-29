using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRMailMessageTypeOverriddenConfiguration : OverriddenConfigurationExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("66A97FDC-156B-4F87-B12E-89B912D1E74A"); }
        }
        public Guid VRMailMessageTypeId { get; set; }

        public string OverriddenName { get; set; }

        public VRMailMessageTypeSettings OverriddenSettings { get; set; }

        public override Type GetBehaviorType(IOverriddenConfigurationGetBehaviorContext context)
        {
            return typeof(VRMailMessageTypeOverriddenConfigurationBehavior);
        }

        #region Private Methods

        private class VRMailMessageTypeOverriddenConfigurationBehavior : OverriddenConfigurationBehavior
        {
            public override void GenerateScript(IOverriddenConfigurationBehaviorGenerateScriptContext context)
            {
                VRMailMessageTypeManager mailTypeManager = new VRMailMessageTypeManager();
                List<VRMailMessageType> mailTypes = new List<VRMailMessageType>();
                foreach (var config in context.Configs)
                {
                    VRMailMessageTypeOverriddenConfiguration mailTypeConfig = config.Settings.ExtendedSettings.CastWithValidate<VRMailMessageTypeOverriddenConfiguration>("mailTypeConfig", config.OverriddenConfigurationId);

                    var mailType = mailTypeManager.GetMailMessageType(mailTypeConfig.VRMailMessageTypeId);
                    mailType.ThrowIfNull("mailType", mailTypeConfig.VRMailMessageTypeId);
                    mailType.Settings.ThrowIfNull("mailType.Settings", mailTypeConfig.VRMailMessageTypeId);
                    mailType = mailType.VRDeepCopy();
                    if (!String.IsNullOrEmpty(mailTypeConfig.OverriddenName))
                        mailType.Name = mailTypeConfig.OverriddenName;
                    if (mailTypeConfig.OverriddenSettings != null)
                        mailType.Settings = mailTypeConfig.OverriddenSettings;
                    mailTypes.Add(mailType);                   
                }
                GenerateScript(mailTypes, context.AddEntityScript);
            }

            public override void GenerateDevScript(IOverriddenConfigurationBehaviorGenerateDevScriptContext context)
            {
                IEnumerable<Guid> ids = context.Configs.Select(config => config.Settings.ExtendedSettings.CastWithValidate<VRMailMessageTypeOverriddenConfiguration>("config.Settings.ExtendedSettings", config.OverriddenConfigurationId).VRMailMessageTypeId).Distinct();
                VRMailMessageTypeManager mailTypeManager = new VRMailMessageTypeManager();
                List<VRMailMessageType> mailTypes = new List<VRMailMessageType>();
                foreach(var id in ids)
                {
                    var mailType = mailTypeManager.GetMailMessageType(id);
                    mailType.ThrowIfNull("mailType", id);
                    mailTypes.Add(mailType);
                }
                GenerateScript(mailTypes, context.AddEntityScript);
            }

            private void GenerateScript(List<VRMailMessageType> mailTypes, Action<string, string> addEntityScript)
            {
                IVRMailMessageTypeDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRMailMessageTypeDataManager>();
                dataManager.GenerateScript(mailTypes, addEntityScript);
            }
        }

        #endregion
    }
}
