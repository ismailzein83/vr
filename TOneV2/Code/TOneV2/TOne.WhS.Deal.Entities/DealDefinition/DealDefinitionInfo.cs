
using System;

namespace TOne.WhS.Deal.Entities
{
    public class DealDefinitionInfo
    {
        public int DealId { get; set; }

        public string Name { get; set; }

        public Guid ConfigId { get; set; }

        public int? SellingNumberPlanId { get; set; }
    }
}
