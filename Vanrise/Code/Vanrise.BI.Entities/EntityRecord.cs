using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BI.Entities
{
    public class EntityRecord
    {
        public List<string> EntityId { get; set; }

        public List<string> EntityName { get; set; }

        public List<string> EntityType { get; set; }

        public Decimal[] Values { get; set; }
    }
}
