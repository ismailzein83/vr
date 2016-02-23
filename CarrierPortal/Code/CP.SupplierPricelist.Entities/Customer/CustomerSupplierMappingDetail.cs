using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP.SupplierPricelist.Entities
{
    public class CustomerSupplierMappingDetail
    {

        public CustomerSupplierMapping Entity { get; set; }
        public string UserName { get; set; }
        public string SupplierNames { get; set; }
    }
}
