using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP.WhS.Entities
{
    public class ClientAccountInfoFilter
    {
        public bool GetCustomers { get; set; }
        public bool GetSuppliers { get; set; }
        public Guid? BusinessEntityDefinitionId { get; set; }
    }
}
