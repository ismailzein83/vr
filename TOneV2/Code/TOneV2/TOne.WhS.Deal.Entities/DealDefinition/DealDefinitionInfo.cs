using System;

namespace TOne.WhS.Deal.Entities
{
    public enum DealDefinitionInfoStatus { Active = 0, Inactive = 1, Draft = 2, Deleted = 3 }
    public class DealDefinitionInfo
    {
        public int DealId { get; set; }

        public string Name { get; set; }

        public Guid ConfigId { get; set; }

        public int? SellingNumberPlanId { get; set; }

        public DealDefinitionInfoStatus DealDefinitionInfoStatus { get; set; }

        public bool IsForced { get; set; }
    }
}