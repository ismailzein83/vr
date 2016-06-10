﻿using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public abstract class ChargingPolicyDurationTariff : ChargingPolicyPart
    {        
        public int ConfigId { get; set; }

        public abstract void Execute(IChargingPolicyDurationTariffContext context);

        public override string PartTypeName
        {
            get { return "Retail_BE_ChargingPolicyPart_DurationTariff"; }
        }
    }

    public interface IChargingPolicyDurationTariffContext : IChargingPolicyPartExecutionContext
    {
        DateTime? TargetTime { get; }

        Decimal Rate { get; }

        Decimal? DurationInSeconds { get; }

        Decimal EffectiveRate { set; }

        Decimal? EffectiveDurationInSeconds { set; }

        Decimal? TotalAmount { set; }
    }
}
