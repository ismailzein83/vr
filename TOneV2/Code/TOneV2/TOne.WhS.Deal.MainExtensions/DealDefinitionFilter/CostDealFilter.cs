using TOne.WhS.Deal.Entities;
using Vanrise.Common;

namespace TOne.WhS.Deal.MainExtensions.DealDefinitionFilter
{
    public class CostDealFilter : IDealDefinitionFilter
    {
        public bool IsMatched(IDealDefinitionFilterContext context)
        {
            context.DealDefinition.ThrowIfNull("context.DealDefinition");
            context.DealDefinition.Settings.ThrowIfNull("context.DealDefinition.Settings");

            if (context.DealDefinition.Settings.GetDealZoneGroupPart() == DealZoneGroupPart.Sale)
                return false;

            return true;
        }
    }
}
