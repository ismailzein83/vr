using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP.SupplierPricelist.Entities
{
    public class Customer
    {
        public int CustomerId { get; set; }

        public string Name { get; set; }

        public CustomerSettings Settings { get; set; }
    }

    public class CustomerQuery
    {

        public string Name { get; set; }
    }
}
