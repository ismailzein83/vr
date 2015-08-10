using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class CarrierGroup
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int? ParentID { get; set; }
    }
}
