using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Entities;
using Vanrise.Common;

namespace TOne.WhS.Deal.Business.VolumeCommitment
{
    public class VolCommitmentDealFilter : IDealDefinitionFilter
    {
        public virtual bool IsMatched(IDealDefinitionFilterContext context)
        {
            context.DealDefinition.ThrowIfNull("context.DealDefinition");
            context.DealDefinition.Settings.ThrowIfNull("context.DealDefinition.Settings");
            if (context.DealDefinition.Settings.ConfigId != VolCommitmentDealSettings.VolCommitmentDealSettingsConfigId)
                return false;

            return true;
        }
    }

    public class VolCommitmentBuyDealFilter : VolCommitmentDealFilter
    {
        public override bool IsMatched(IDealDefinitionFilterContext context)
        {
            context.DealDefinition.ThrowIfNull("context.DealDefinition");
            context.DealDefinition.Settings.ThrowIfNull("context.DealDefinition.Settings");
            if (context.DealDefinition.Settings.ConfigId != VolCommitmentDealSettings.VolCommitmentDealSettingsConfigId)
                return false;

            VolCommitmentDealSettings volCommitmentDealSettings = context.DealDefinition.Settings.CastWithValidate<VolCommitmentDealSettings>("VolCommitmentDealSettings", VolCommitmentDealSettings.VolCommitmentDealSettingsConfigId);

            if (volCommitmentDealSettings.DealType != VolCommitmentDealType.Buy)
                return false;

            return true;
        }
    }

    public class VolCommitmentSellDealFilter : VolCommitmentDealFilter
    {
        public override bool IsMatched(IDealDefinitionFilterContext context)
        {
            context.DealDefinition.ThrowIfNull("context.DealDefinition");
            context.DealDefinition.Settings.ThrowIfNull("context.DealDefinition.Settings");
            if (context.DealDefinition.Settings.ConfigId != VolCommitmentDealSettings.VolCommitmentDealSettingsConfigId)
                return false;

            VolCommitmentDealSettings volCommitmentDealSettings = context.DealDefinition.Settings.CastWithValidate<VolCommitmentDealSettings>("VolCommitmentDealSettings", VolCommitmentDealSettings.VolCommitmentDealSettingsConfigId);

            if (volCommitmentDealSettings.DealType != VolCommitmentDealType.Sell)
                return false;

            return true;
        }
    }
}
