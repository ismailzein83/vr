using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Entities;
using Vanrise.Common;

namespace TOne.WhS.Deal.Business
{
    class DealVolBuyAndSwapFilter : IDealDefinitionFilter
    {
        public bool IsMatched(IDealDefinitionFilterContext context)
        {
            context.DealDefinition.ThrowIfNull("context.DealDefinition");
            context.DealDefinition.Settings.ThrowIfNull("context.DealDefinition.Settings");
            if (context.DealDefinition.Settings.ConfigId != VolCommitmentDealSettings.VolCommitmentDealSettingsConfigId && context.DealDefinition.Settings.ConfigId != SwapDealSettings.SwapDealSettingsConfigId)
                return false;

            VolCommitmentDealSettings volCommitmentDealSettings = context.DealDefinition.Settings.CastWithValidate<VolCommitmentDealSettings>("VolCommitmentDealSettings", VolCommitmentDealSettings.VolCommitmentDealSettingsConfigId);

            if (volCommitmentDealSettings.DealType != VolCommitmentDealType.Buy)
                return false;

            return true;
        }
    }
}
