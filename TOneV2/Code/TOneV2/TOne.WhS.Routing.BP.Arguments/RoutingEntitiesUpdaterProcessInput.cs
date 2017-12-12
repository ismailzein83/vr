using System;
using System.Collections.Generic;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.BP.Arguments
{
    public class RoutingEntitiesUpdaterProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public bool RebuildCustomerZoneDetails { get; set; }
        public bool RebuildSupplierZoneDetails { get; set; }

        public DateTime? NextOpenOrCloseSaleRateTime { get; set; }
        public object MaxSaleRateTimeStamp { get; set; }

        public DateTime? NextOpenOrCloseSupplierRateTime { get; set; }
        public object MaxSupplierRateTimeStamp { get; set; }

        public override string GetTitle()
        {
            return String.Format("#BPDefinitionTitle#");
        }
    }
}