using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class StatusDefinition
    {
        public Guid StatusDefinitionId { get; set; }
        public string Name { get; set; }
        public Guid BusinessEntityDefinitionId { get; set; }
        public StatusDefinitionSettings Settings { get; set; }
    }

    public class StatusDefinitionSettings
    {
        public Guid StyleDefinitionId { get; set; }

        public bool HasInitialCharge { get; set; }

        public bool HasRecurringCharge { get; set; }
    }
}
