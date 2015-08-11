using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class CarrierGroupQuery
    {
        public int GroupId { get; set; }
        public bool WithDescendants { get; set; }
    }
}
