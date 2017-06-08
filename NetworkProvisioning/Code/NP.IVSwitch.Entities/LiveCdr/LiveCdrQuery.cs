using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities
{
     public enum CallsMode {None=0, OlderThem = 1, NewerThem = 2}
    public enum TimeUnit {None=0, Seconds = 1, Minutes = 2}
   public  class LiveCdrQuery
    {
      public List<int>  EndPointIds { get;set;}
      public List<int> RouteIds { get; set; }
      public string SourceIP { get; set; }
      public string RouteIP { get; set; }
      public CallsMode CallsMode { get; set; }
      public TimeUnit TimeUnit { get; set; }
      public double Time { get; set; }
    }
}
