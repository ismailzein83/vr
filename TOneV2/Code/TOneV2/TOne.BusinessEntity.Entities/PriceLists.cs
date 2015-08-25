using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class PriceLists
    {
        public int PriceListID { get; set; }
        public string Description { get; set; }
        public string SourceFileName { get; set; }
        public int UserID { get; set; }
         public DateTime BeginEffectiveDate { get; set; }
         public string Name { get; set; }
        
            
    }
}
