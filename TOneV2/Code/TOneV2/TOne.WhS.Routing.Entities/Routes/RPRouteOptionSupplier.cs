using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public enum SupplierStatus { Active = 0, PartialActive = 1, Block = 2 }

    public class RPRouteOptionSupplier
    {
        static RPRouteOptionSupplier()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(RPRouteOptionSupplier),
                "SupplierId", "SupplierZones");
        }

        public int SupplierId { get; set; }

        public List<RPRouteOptionSupplierZone> SupplierZones { get; set; }

        public SupplierStatus SupplierStatus
        {
            get
            {
                if (NumberOfBlockedZones == 0)
                    return Entities.SupplierStatus.Active;
                if (NumberOfUnblockedZones == 0)
                    return Entities.SupplierStatus.Block;
                return Entities.SupplierStatus.PartialActive;
            }
        }

        public int NumberOfBlockedZones { get; set; }
        public int NumberOfUnblockedZones { get; set; }

        public decimal? Percentage { get; set; }
    }
}
