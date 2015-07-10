using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class CarrierGroup
    {
        public int CarrierGroupID { get; set; }
        public string CarrierGroupName { get; set; }
        public int? ParentID { get; set; }
        public string ParentPath { get; set; }
        public string Path { get; set; }
    }
}
