using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                StringBuilder scriptBuilder = new StringBuilder();
                VRMailMessageTypeManager mailTypeManager = new VRMailMessageTypeManager();
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
                    if (scriptBuilder.Length > 0)
                    {
                        scriptBuilder.Append(",");
                        scriptBuilder.AppendLine();
                    }
                    scriptBuilder.AppendFormat(@"('{0}','{1}','{2}')", mailType.VRMailMessageTypeId, mailType.Name, Serializer.Serialize(mailType.Settings));
                }
                string script = String.Format(@"set nocount on;;with cte_data([ID],[Name],[Settings])as (select * from (values--//////////////////////////////////////////////////////////////////////////////////////////////////{0}--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\)c([ID],[Name],[Settings]))merge	[common].[MailMessageType] as tusing	cte_data as son		1=1 and t.[ID] = s.[ID]when matched then	update set	[Name] = s.[Name],[Settings] = s.[Settings]when not matched by target then	insert([ID],[Name],[Settings])	values(s.[ID],s.[Name],s.[Settings]);", scriptBuilder);
                context.AddEntityScript("[common].[MailMessageType]", script);
            }
        }

        #endregion
    }
}
