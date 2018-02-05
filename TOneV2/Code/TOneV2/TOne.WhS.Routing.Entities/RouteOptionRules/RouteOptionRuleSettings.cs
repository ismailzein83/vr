using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Rules;

namespace TOne.WhS.Routing.Entities
{
    public abstract class RouteOptionRuleSettings
    {
        public abstract Guid ConfigId { get; }


        public abstract void Execute(IRouteOptionRuleExecutionContext context, BaseRouteOptionRuleTarget target);

        public virtual RouteOptionRuleSettingsEditorRuntime GetEditorRuntime()
        {
            return null;
        }

        public virtual void RefreshState(IRefreshRuleStateContext context) { }

        public abstract RouteOptionRuleSettings BuildLinkedRouteOptionRuleSettings(ILinkedRouteOptionRuleContext context);
    }

    public abstract class RouteOptionRuleSettingsEditorRuntime
    {

    }
}
