using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public enum RoutingDatabaseType { Current = 0, Future = 1, SpecificDate = 2 }
    public class RoutingDatabase
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public RoutingDatabaseType Type { get; set; }
        public DateTime EffectiveTime { get; set; }
        public bool IsReady { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime ReadyTime { get; set; }
    }
}
