using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Security.Business;

namespace Vanrise.GenericData.Business
{
    public class GenericRuleDefinitionViewFilter : IGenericRuleDefinitionFilter
    {
        public Guid ViewId { get; set; }

        public  bool IsMatched(IGenericRuleDefinitionFilterContext context)
        {
            ViewManager viewManager = new ViewManager();
            var genericRuleViewSettings = viewManager.GetView(this.ViewId).Settings as GenericRuleViewSettings;
            GenericRuleDefinitionManager genericRuleDefinitionManager = new GenericRuleDefinitionManager();
            if (!genericRuleViewSettings.RuleDefinitionIds.Contains(context.RuleDefinition.GenericRuleDefinitionId))
                return false;
            if (!genericRuleDefinitionManager.DoesUserHaveViewAccess(context.RuleDefinition))
                return false;

            return true ;
        }
    }
}
