using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BI.Entities
{
    public enum EntityType
    {
        SaleZone = 0,
        Customer = 1,
        Supplier = 2
    }
    public class GenericEntityRecord
    {
        public string EntityId { get; set; }

        public string EntityName { get; set; }

        public EntityType EntityType { get; set; }

        public Decimal Value { get; set; }
    }
}
