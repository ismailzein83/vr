using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class EntityPersonalization
    {
        public long EntityPersonalizationId { get; set; }
        public int? UserId { get; set; }
        public string EntityUniqueName { get; set; }
        public EntityPersonalizationData Details { get; set; }
        public DateTime? CreatedTime { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? LastModifiedTime { get; set; }
        public int? LastModifiedBy { get; set; }

    }
}
