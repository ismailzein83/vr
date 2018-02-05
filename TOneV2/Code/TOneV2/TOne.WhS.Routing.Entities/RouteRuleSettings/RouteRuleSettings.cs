using System;
using System.Collections.Generic;

namespace TOne.WhS.Routing.Entities
{
    public enum CorrespondentType { Other, SpecialRequest, Block, LCR, Override }
    public abstract class RouteRuleSettings
    {
        public abstract Guid ConfigId { get; }

        public virtual CorrespondentType CorrespondentType { get { return Entities.CorrespondentType.Other; } }

        public virtual bool UseOrderedExecution { get { return false; } }

        public virtual List<RouteOptionRuleTarget> GetOrderedOptions(ISaleEntityRouteRuleExecutionContext context, RouteRuleTarget target)
        {
            return null;
        }

        public virtual void GetQualityConfigurationIds(IRouteRuleQualityContext context)
        {
        }

        public virtual void CheckOptionFilter(ISaleEntityRouteRuleExecutionContext context, RouteRuleTarget target, BaseRouteOptionRuleTarget option)
        {
        }

        public virtual void ApplyOptionsPercentage(IEnumerable<RouteOption> options)
        {

        }

        public abstract void ExecuteForSaleEntity(ISaleEntityRouteRuleExecutionContext context, RouteRuleTarget target);

        public abstract void CreateSupplierZoneOptionsForRP(IRPRouteRuleExecutionContext context, RouteRuleTarget target);

        public abstract void ApplyRuleToRPOptions(IRPRouteRuleExecutionContext context, ref IEnumerable<RPRouteOption> options);

        public virtual int? GetMaxNumberOfOptions(ISaleEntityRouteRuleExecutionContext context)
        {
            return context.NumberOfOptions;
        }

        public virtual List<RouteOption> GetFinalOptions(IFinalizeRouteOptionContext context)
        {
            return context.RouteOptions;
        }

        public abstract RouteRuleSettings BuildLinkedRouteRuleSettings(ILinkedRouteRuleContext context);
    }
}
