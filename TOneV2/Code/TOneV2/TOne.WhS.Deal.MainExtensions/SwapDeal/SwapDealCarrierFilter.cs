using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using Vanrise.Common;

namespace TOne.WhS.Deal.MainExtensions.SwapDeal
{
    public class SwapDealCarrierFilter : IDealDefinitionFilter
    {
        public int CarrierId { get; set; }
        public bool IsMatched(IDealDefinitionFilterContext context)
        {
            context.DealDefinition.ThrowIfNull("context.DealDefinition");
            context.DealDefinition.Settings.ThrowIfNull("context.DealDefinition.Settings");

            if (context.DealDefinition.Settings.ConfigId != SwapDealSettings.SwapDealSettingsConfigId)
                return false;
            if (context.DealDefinition.Settings.GetCarrierAccountId() != CarrierId)
                return false;
            return true;
        }
    }
}
