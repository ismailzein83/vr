using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.Deal.Entities
{
   public class DealZoneInfo 
   {
       public long ZoneId {get;set;}
       public int DealId { get; set; }
       public DateTime BED { get; set; }
       public DateTime? EED { get; set; }

   }
}
