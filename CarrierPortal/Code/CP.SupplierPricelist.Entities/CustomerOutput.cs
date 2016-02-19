using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP.SupplierPricelist.Entities
{
    public class CustomerOutput
    {
        public List<Customer> Customers { get; set; }
        public byte[] MaxTimeStamp { get; set; }
    }
}
