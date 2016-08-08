using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class DraftRateToClose
    {
        public long RateId { get; set; }
        public int? RateTypeId { get; set; }
        public DateTime EED { get; set; }
    }
}
