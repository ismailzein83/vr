using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using Vanrise.Common;

namespace TOne.WhS.Deal.MainExtensions.SwapDeal
{
    public class SwapDealDefinitionFilter : IDealDefinitionFilter
    {
        public bool IsMatched(IDealDefinitionFilterContext context)
        {
            context.DealDefinition.ThrowIfNull("context.DealDefinition");
            context.DealDefinition.Settings.ThrowIfNull("context.DealDefinition.Settings");

            if (context.DealDefinition.Settings.ConfigId != SwapDealSettings.SwapDealSettingsConfigId)
                return false;

            return true;
        }
    }
}
