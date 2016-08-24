using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class EntityStatusChargeInfo
    {
        public EntityType EntityType { get; set; }

        /// <summary>
        /// this could be AccountTypeId or ServiceTypeId
        /// </summary>
        public string EntityTypeId { get; set; }

        public Guid StatusDefinitionId { get; set; }

        public string StatusName { get; set; }

        public bool HasInitialCharge { get; set; } 

        public bool HasRecurringCharge { get; set; }
    }
}
