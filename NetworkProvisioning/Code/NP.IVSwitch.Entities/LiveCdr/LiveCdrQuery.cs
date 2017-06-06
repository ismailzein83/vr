using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities
{
   public  class LiveCdrQuery
    {
      public List<int>  EndPointIds { get;set;}
      public List<int> RouteIds { get; set; }
      public string SourceIP { get; set; }
      public string RouteIP { get; set; }
    }
}
