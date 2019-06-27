﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.Entities
{
    public class SwapDealAnalysisDefinition
    {
        public int SwapDealAnalysisDefinitionId { get; set; }

        public string Name { get; set; }

        public SwapDealAnalysisSettings Settings { get; set; }
    }

    public class SwapDealAnalysisSettings
    {
        public int? SwapDealId { get; set; }

        public int CarrierAccountId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public List<SwapDealAnalysisInboundSettings> Inbounds { get; set; }

        public List<SwapDealAnalysisOutboundSettings> Outbounds { get; set; }
        public AnalysisResultCustomObject AnalysisResult { get; set; }
    }

    public class SwapDealAnalysisOutboundSettings
    {
        public int ZoneGroupNumber { get; set; }
        public string GroupName { get; set; }

        public int CountryId { get; set; }

        public List<long> SupplierZoneIds { get; set; }

        public int Volume { get; set; }

        public Decimal DealRate { get; set; }

        public SwapDealAnalysisOutboundItemRateCalcMethod ItemCalculationMethod { get; set; }
        public Guid CalculationMethodId { get; set; }
    }

    public class SwapDealAnalysisInboundSettings
    {
        public int ZoneGroupNumber { get; set; }
        public string GroupName { get; set; }

        public int CountryId { get; set; }

        public List<long> SaleZoneIds { get; set; }

        public int Volume { get; set; }

        public Decimal DealRate { get; set; }

        public SwapDealAnalysisInboundItemRateCalcMethod ItemCalculationMethod { get; set; }
        public Guid CalculationMethodId { get; set; }
    }
}
