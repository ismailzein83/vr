using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class ServiceType
    {
        public const string BUSINESSENTITY_DEFINITION_NAME = "Retail_BE_ServiceType";

        public Guid ServiceTypeId { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public Guid AccountBEDefinitionId { get; set; }

        public ServiceTypeSettings Settings { get; set; }
    }

    
}
