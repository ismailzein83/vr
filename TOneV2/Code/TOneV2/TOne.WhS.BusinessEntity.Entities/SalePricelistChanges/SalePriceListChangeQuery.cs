using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SalePriceListChangeQuery
    {
        public int PriceListId { get; set; }
        public List<int> Countries { get; set; }
    }
}
