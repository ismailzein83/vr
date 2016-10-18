using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.BP.Arguments
{
    public class RPRoutingProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public DateTime? EffectiveOn { get; set; }
        public RoutingDatabaseType RoutingDatabaseType { get; set; }
        public RoutingProcessType RoutingProcessType { get; set; }
        public bool IsFuture { get; set; }
        public int SaleZoneRange { get; set; }
        public List<SupplierZoneToRPOptionPolicy> SupplierZoneRPOptionPolicies { get; set; }
        public bool IncludeBlockedSupplierZones { get; set; }

        public override string GetTitle()
        {
            return String.Format("#BPDefinitionTitle# for Effective Time {0}", EffectiveOn);
        }
    }
}
