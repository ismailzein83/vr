using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public abstract class SupplierZoneToRPOptionPolicy
    {
        public Guid ConfigId { get; set; }
        public bool IsDefault { get; set; }
        public abstract void Execute(ISupplierZoneToRPOptionPolicyExecutionContext context);
    }    
}
