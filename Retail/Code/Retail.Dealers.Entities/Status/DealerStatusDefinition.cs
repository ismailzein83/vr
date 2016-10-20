using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Dealers.Entities
{
    public class DealerStatusDefinition
    {
        public Guid DealerStatusDefinitionId { get; set; }

        public string Name { get; set; }

        public DealerStatusDefinitionSettings Settings { get; set; }
    }

    public class DealerStatusDefinitionSettings
    {
        public Guid StyleDefinitionId { get; set; }
    }
}
