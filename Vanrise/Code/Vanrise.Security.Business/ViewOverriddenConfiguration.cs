using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class ViewOverriddenConfiguration : OverriddenConfigurationExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("3ADC69D8-81AD-41EF-87B2-301E29F4DF1F"); }
        }

        public Guid ViewId { get; set; }

        public string OverriddenName { get; set; }

        public string OverriddenTitle { get; set; }

        public bool IsModuleIdOverridden { get; set; }

        public Guid? OverriddenModuleId { get; set; }

        public bool IsAudienceOverridden { get; set; }

        public AudienceWrapper OverriddenAudience { get; set; }

        public int? OverriddenRank { get; set; }

        public ViewSettings OverriddenSettings { get; set; }

        public override Type GetBehaviorType(IOverriddenConfigurationGetBehaviorContext context)
        {
            return typeof(ViewOverriddenConfigurationBehavior);
        }

        #region Private Classes

        private class ViewOverriddenConfigurationBehavior : OverriddenConfigurationBehavior
        {
            public override void GenerateScript(IOverriddenConfigurationBehaviorGenerateScriptContext context)
            {
                StringBuilder scriptBuilder = new StringBuilder();
                ViewManager viewManager = new ViewManager();
                foreach (var config in context.Configs)
                {
                    ViewOverriddenConfiguration viewConfig = config.Settings.ExtendedSettings.CastWithValidate<ViewOverriddenConfiguration>("viewConfig", config.OverriddenConfigurationId);

                    var view = viewManager.GetView(viewConfig.ViewId);
                    view.ThrowIfNull("view", viewConfig.ViewId);
                    view = view.VRDeepCopy();
                    if (!String.IsNullOrEmpty(viewConfig.OverriddenName))
                        view.Name = viewConfig.OverriddenName;
                    if (!String.IsNullOrEmpty(viewConfig.OverriddenTitle))
                        view.Title = viewConfig.OverriddenTitle;
                    if (viewConfig.IsModuleIdOverridden)
                        view.ModuleId = viewConfig.OverriddenModuleId;
                    if (viewConfig.IsAudienceOverridden)
                        view.Audience = viewConfig.OverriddenAudience;
                    if (viewConfig.OverriddenRank.HasValue)
                        view.Rank = viewConfig.OverriddenRank.Value;
                    if (viewConfig.OverriddenSettings != null)
                        view.Settings = viewConfig.OverriddenSettings;
                    if (scriptBuilder.Length > 0)
                    {
                        scriptBuilder.Append(",");
                        scriptBuilder.AppendLine();
                    }
                    scriptBuilder.AppendFormat(@"('{0}','{1}','{2}',{3},{4},{5},{6},{7},{8},{9},{10})", 
                        view.ViewId, view.Name, view.Title, GetStringSQLValue(view.Url), GetStringSQLValue(view.ModuleId.HasValue ? view.ModuleId.Value.ToString() : null), GetStringSQLValue(view.ActionNames),
                        GetStringSQLValue(view.Audience != null ? Serializer.Serialize(view.Audience) : null), GetStringSQLValue(view.ViewContent != null ? Serializer.Serialize(view.ViewContent) : null),
                        GetStringSQLValue(view.Settings != null ? Serializer.Serialize(view.Settings) : null), GetStringSQLValue(view.Type.ToString()), view.Rank);
                }
                string script = String.Format(@"set nocount on;
;with cte_data([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
{0}--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank]))
merge	[sec].[View] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Url] = s.[Url],[Module] = s.[Module],[ActionNames] = s.[ActionNames],[Audience] = s.[Audience],[Content] = s.[Content],[Settings] = s.[Settings],[Type] = s.[Type],[Rank] = s.[Rank]
when not matched by target then
	insert([ID],[Name],[Title],[Url],[Module],[ActionNames],[Audience],[Content],[Settings],[Type],[Rank])
	values(s.[ID],s.[Name],s.[Title],s.[Url],s.[Module],s.[ActionNames],s.[Audience],s.[Content],s.[Settings],s.[Type],s.[Rank]);", scriptBuilder);
                context.AddEntityScript("[sec].[View]", script);
            }

            private string GetStringSQLValue(string value)
            {
                return value != null ? string.Format("'{0}'", value) : "null";
            }
        }

        #endregion
    }
}
