using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities.RatePlanning.Input
{
    public class SaveChangesInput
    {
        public RatePlanOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public Changes Changes { get; set; }
    }
}
