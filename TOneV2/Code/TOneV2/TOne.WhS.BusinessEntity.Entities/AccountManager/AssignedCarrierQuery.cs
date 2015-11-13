using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class AssignedCarrierQuery
    {
        public int ManagerId { get; set; }

        public bool WithDescendants { get; set; }
    }
}
