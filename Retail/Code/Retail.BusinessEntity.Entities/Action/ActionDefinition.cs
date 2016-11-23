using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace Retail.BusinessEntity.Entities
{
    public class ActionDefinition
    {
        public Guid ActionDefinitionId { get; set; }

        public string Name { get; set; }
        public EntityType EntityType { get; set; }

        public ActionDefinitionSettings Settings { get; set; }
    }

    public class ActionDefinitionSettings
    {
        public string Description { get; set; }
        
        /// <summary>
        /// = ServiceTypeId in case EntityType = EntityType.AccountService
        /// </summary>
        public Guid? EntityTypeId { get; set; }

        public AccountCondition AvailabilityCondition { get; set; }

        public AccountServiceCondition ServiceAvailabilityCondition { get; set; }

        public List<ActionStatusDefinition> SupportedOnStatuses { get; set; }

        public ActionBPDefinitionSettings BPDefinitionSettings { get; set; }
    }

    public class ActionStatusDefinition
    {
        public Guid StatusDefinitionId { get; set; }
    }
}
