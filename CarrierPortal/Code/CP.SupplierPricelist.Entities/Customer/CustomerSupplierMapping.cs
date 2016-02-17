using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP.SupplierPricelist.Entities
{
    public class CustomerSupplierMapping
    {
        public int CustomerId { get; set; }

        public int UserId { get; set; }

        public CustomerSupplierMappingSettings Settings { get; set; }
    }
}
