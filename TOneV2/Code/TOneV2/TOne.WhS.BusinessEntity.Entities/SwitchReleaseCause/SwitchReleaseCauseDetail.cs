using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SwitchReleaseCauseDetail
    {
        public int SwitchReleaseCauseId { get; set; }
        public string ReleaseCode { get; set; }
        public string SwitchName { get; set; }
        public int SwitchId { get; set; }
        public string Description { get; set; }
        public bool IsDelivered { get; set; }
    }
}
