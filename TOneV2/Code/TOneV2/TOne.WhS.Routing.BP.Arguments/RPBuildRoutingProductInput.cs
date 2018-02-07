using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.BP.Arguments
{
    public class RPBuildRoutingProductInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public long FromZoneId { get; set; }
        public long ToZoneId { get; set; }
        public int RoutingDatabaseId { get; set; }
        public List<SupplierZoneToRPOptionPolicy> SupplierOptionPolicies { get; set; }
        public DateTime? EffetiveDate { get; set; }
        public bool IsFuture { get; set; }

        public override string GetTitle()
        {
            return string.Format("#BPDefinitionTitle# for Zones: {0} to {1}", FromZoneId, ToZoneId);
        }
    }
}
