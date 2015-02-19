using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public enum RoutingDatabaseType { Current, Future}

    public class RoutingDatabase
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public RoutingDatabaseType Type { get; set; }

        public string TypeInfo { get; set; }

        public DateTime BeginEffectiveDate { get; set; }

        public DateTime? EndEffectiveDate { get; set; }

        public bool IsReady { get; set; }
    }
}
