using System.Collections.Generic;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SellingProductQuery
    {
        public string Name { get; set; }
        public List<int> SellingNumberPlanIds { get; set; }
    }
}
