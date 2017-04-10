using System;
using System.Collections.Generic;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SalePriceListQuery
    {
        public IEnumerable<int> OwnerId { get; set; }
        public DateTime? CreationDate { get; set; }
    }
}
