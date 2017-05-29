using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.Security.Entities;
using Vanrise.Security.Data;

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
               ViewManager viewManager = new ViewManager();
                List<View> views = new List<View>();
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
                    views.Add(view);                    
                }
                GenerateScript(views, context.AddEntityScript);
            }

            public override void GenerateDevScript(IOverriddenConfigurationBehaviorGenerateDevScriptContext context)
            {
                IEnumerable<Guid> ids = context.Configs.Select(config => config.Settings.ExtendedSettings.CastWithValidate<ViewOverriddenConfiguration>("config.Settings.ExtendedSettings", config.OverriddenConfigurationId).ViewId).Distinct();
                ViewManager viewManager = new ViewManager();
                List<View> views = new List<View>();
                foreach (var id in ids)
                {
                    var view = viewManager.GetView(id);
                    view.ThrowIfNull("view", id);
                    views.Add(view); 
                }
                GenerateScript(views, context.AddEntityScript);
            }

            private void GenerateScript(List<View> views, Action<string, string> addEntityScript)
            {
                IViewDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IViewDataManager>();
                dataManager.GenerateScript(views, addEntityScript);
            }
        }

        #endregion
    }
}
