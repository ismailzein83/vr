﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.Deal.Entities
{
    public class SwapDealAnalysisOutboundRateCalcMethodConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "WhS_Deal_SwapDeal_OutboundRateCalculationMetod";
        
        public string Editor { get; set; }
    }
}
