using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Dealers.Entities
{
    public class DealerActionDefinition
    {
        public Guid DealerActionDefinitionId { get; set; }

        public string Name { get; set; }

        public DealerActionDefinitionSettings Settings { get; set; }
    }

    public class DealerActionDefinitionSettings
    {
        public string Description { get; set; }

        public List<DealerActionStatusDefinition> SupportedOnStatuses { get; set; }

        public DealerActionBPDefinitionSettings BPDefinitionSettings { get; set; }
    }

    public class DealerActionStatusDefinition
    {
        public Guid StatusDefinitionId { get; set; }
    }
}
