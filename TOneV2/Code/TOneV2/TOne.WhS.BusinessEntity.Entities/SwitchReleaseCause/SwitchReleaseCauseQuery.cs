using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SwitchReleaseCauseQuery
    {
        public string ReleaseCode { get; set; }
        public List<int> SwitchIds { get; set; }
        public bool? IsDelivered { get; set; }
        public string Description { get; set; }
    }

}
