using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public enum RoutingProcessType : byte { RoutingProductRoute = 0, CustomerRoute = 1 }
    public enum RoutingDatabaseType : byte { Current = 0, Future = 1, SpecificDate = 2 }
    public class RoutingDatabase
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public RoutingProcessType ProcessType { get; set; }
        public RoutingDatabaseType Type { get; set; }
        public DateTime? EffectiveTime { get; set; }
        public bool IsReady { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime ReadyTime { get; set; }
        public RoutingDatabaseInformation Information { get; set; }
        public RoutingDatabaseSettings Settings { get; set; }
    }

    public abstract class RoutingDatabaseInformation
    {
    }

    public class RPRoutingDatabaseInformation : RoutingDatabaseInformation
    {
        public Guid DefaultPolicyId { get; set; }
        public Guid[] SelectedPoliciesIds { get; set; }
    }

    public class RoutingDatabaseSettings
    {
        public string DatabaseName { get; set; }
    }
}