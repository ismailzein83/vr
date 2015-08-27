using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
   public class RateAnalysisQuery
    {
       public int ZoneId { get; set; }
       public DateTime EffectedDate { get; set; }
       public string CustomerId { get; set; }
       public string SupplierId { get; set; }
    }
}
