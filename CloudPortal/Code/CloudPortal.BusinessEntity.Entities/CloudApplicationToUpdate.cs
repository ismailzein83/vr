using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPortal.BusinessEntity.Entities
{
    public class CloudApplicationToUpdate
    {
        public int CloudApplicationId { get; set; }
        
        public int CloudApplicationTypeId { get; set; }

        public CloudApplicationSettings Settings { get; set; }
    }
}
