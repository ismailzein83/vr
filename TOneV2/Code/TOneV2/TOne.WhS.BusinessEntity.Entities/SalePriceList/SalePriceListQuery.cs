using System;
using System.Collections.Generic;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SalePriceListQuery
    {
        public int? OwnerId { get; set; }
        public DateTime? CreationDate { get; set; }
        public IEnumerable<int> IncludedSalePriceListIds { get; set; }
        public IEnumerable<SalePriceListType> SalePricelistTypes { get; set; }
        public List<int> UserIds { get; set; }
    }
}
