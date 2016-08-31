using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.Routing.BP.Arguments
{
    public class BuildSupplierZoneDetailsInput : BuildCarrierZoneDetailsInput
    {
        public int RoutingDatabaseId { get; set; }

        public DateTime? EffectiveOn { get; set; }

        public bool IsFuture { get; set; }
    }
}