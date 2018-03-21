using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SwitchReleaseCause
    {
        public int SwitchReleaseCauseId { get; set; }
        public int SwitchId { get; set; }
        public string ReleaseCode { get; set; }
        public SwitchReleaseCauseSetting Settings { get; set; }
        public string SourceId { get; set; }

        public DateTime CreatedTime { get; set; }

        public int? CreatedBy { get; set; }

        public int? LastModifiedBy { get; set; }

        public DateTime? LastModifiedTime { get; set; }

    }
    public class SwitchReleaseCauseSetting
    {
        public string Description { get; set; }
        public bool IsDelivered { get; set; }
    }
}
