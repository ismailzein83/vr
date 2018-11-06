using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP.WhS.Entities
{
    public class ClientSupplierZoneInfoFilter
    {
        public Guid? BusinessEntityDefinitionId { get; set; }

        public int SupplierId { get; set; }

    }
}
