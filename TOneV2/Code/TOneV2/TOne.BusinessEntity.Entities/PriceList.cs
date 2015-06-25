using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class PriceList
    {
        public int PriceListID { get; set; }
        public string Description { get; set; }
        public string UserName { get; set; }
        public Nullable<DateTime> BeginEffectiveDate { get; set; }
    }
}
