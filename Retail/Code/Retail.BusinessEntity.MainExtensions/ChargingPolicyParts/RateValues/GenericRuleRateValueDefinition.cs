﻿using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.ChargingPolicyParts.RateValues
{
    public class GenericRuleRateValueDefinition : BaseChargingPolicyPartRuleDefinition
    {
        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }
    }
}
