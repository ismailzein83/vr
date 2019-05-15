﻿using System.Collections.Generic;

namespace TOne.WhS.Deal.Entities
{
    public class DealDefinitionFilter
    {
        public List<IDealDefinitionFilter> Filters { get; set; }

        public List<int> CarrierAccountIds { get; set; }

        public List<int> IncludedDealDefinitionIds { get; set; }

        public List<int> ExcludedDealDefinitionIds { get; set; }

        public List<int> SelectedDealDefinitionIds { get; set; }

        public List<DealStatus> DealStatuses { get; set; }
    }

    public interface IDealDefinitionFilter
    {
        bool IsMatched(IDealDefinitionFilterContext context);

    }

    public interface IDealDefinitionFilterContext
    {
        DealDefinition DealDefinition { get; }
    }

    public class DealDefinitionFilterContext : IDealDefinitionFilterContext
    {
        public DealDefinition DealDefinition { get; set; }
    }
}