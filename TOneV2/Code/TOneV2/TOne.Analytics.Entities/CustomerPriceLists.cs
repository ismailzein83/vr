using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class CustomerPriceLists
    {
        public int PriceListID { get; set; }
        public string Description { get; set; }
        public string SourceFileName { get; set; }
        public int UserID { get; set; }
         public DateTime BeginEffectiveDate { get; set; }
         public string Name { get; set; }
        
            
    }
}
