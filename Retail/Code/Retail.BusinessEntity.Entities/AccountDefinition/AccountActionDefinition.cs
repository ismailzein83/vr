using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountActionDefinition
    {
        public Guid AccountActionDefinitionId { get; set; }

        public string Name { get; set; }

        public AccountCondition AvailabilityCondition { get; set; }

        public AccountActionDefinitionSettings ActionDefinitionSettings { get; set; }
    }

    public abstract class AccountActionDefinitionSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract string ClientActionName { get; }
    }
}
