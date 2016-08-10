using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SaleZoneInfoFilter
    {
        public SaleZoneFilterSettings SaleZoneFilterSettings { get; set; }

        public bool GetEffectiveOnly { get; set; }
    }
}
