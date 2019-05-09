﻿using System;

namespace TOne.WhS.Routing.Entities
{
    public class RouteBuildConfiguration
    {
        public CustomerRouteBuildConfiguration CustomerRoute { get; set; }

        public ProductRouteBuildConfiguration ProductRoute { get; set; }

        public IncludedRulesConfiguration IncludedRules { get; set; }
    }

    public class CustomerRouteBuildConfiguration
    {
        public bool KeepBackUpsForRemovedOptions { get; set; }

        public int NumberOfOptions { get; set; }

        public int IndexesCommandTimeoutInMinutes { get; set; }

        public int? MaxDOP { get; set; }
    }

    public class ProductRouteBuildConfiguration
    {
        public bool KeepBackUpsForRemovedOptions { get; set; }

        public int IndexesCommandTimeoutInMinutes { get; set; }

        public int? MaxDOP { get; set; }

        public bool GenerateCostAnalysisByCustomer { get; set; }

        public bool IncludeBlockedZonesInCalculation { get; set; }
    }

    public class IncludedRulesConfiguration
    {
        public bool IncludeRateTypeRules { get; set; }

        public bool IncludeExtraChargeRules { get; set; }

        public bool IncludeTariffRules { get; set; }
    }
}