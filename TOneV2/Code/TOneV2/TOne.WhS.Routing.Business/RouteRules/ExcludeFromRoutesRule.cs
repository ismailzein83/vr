using System;
using System.Collections.Generic;
using TOne.WhS.Routing.Entities;
using System.Linq;

namespace TOne.WhS.Routing.Business
{
    public class ExcludeFromRoutesRule : RouteRuleSettings
    {
        public override Guid ConfigId { get { return new Guid("F966A804-E906-4DBD-8A8F-748805B8FE2E"); } }

        public override void ApplyRuleToRPOptions(IRPRouteRuleExecutionContext context, ref IEnumerable<RPRouteOption> options)
        {
            throw new NotImplementedException();
        }

        public override bool AreSuppliersIncluded(IRouteRuleAreSuppliersIncludedContext context)
        {
            throw new NotImplementedException();
        }

        public override RouteRuleSettings BuildLinkedRouteRuleSettings(ILinkedRouteRuleContext context)
        {
            throw new NotImplementedException();
        }

        public override void CreateSupplierZoneOptionsForRP(IRPRouteRuleExecutionContext context, RouteRuleTarget target)
        {
            throw new NotImplementedException();
        }

        public override void ExecuteForSaleEntity(ISaleEntityRouteRuleExecutionContext context, RouteRuleTarget target)
        {
            throw new NotImplementedException();
        }

        public override string GetSuppliersDescription()
        {
            return null;
        }

        public override bool ShouldCreateRoute()
        {
            return false;
        }

        public override bool SupportPartialRouteBuild()
        {
            return false;
        }
    }
}
