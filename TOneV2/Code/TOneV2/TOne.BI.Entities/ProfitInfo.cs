using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BI.Entities
{
   public  class ProfitInfo : BaseTimeDimensionRecord
    {
        public decimal Cost { get; set; }

        public decimal Sale { get; set; }

        public decimal Profit { get; set; }
    }
}
