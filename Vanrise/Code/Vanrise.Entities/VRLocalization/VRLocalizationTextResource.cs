using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRLocalizationTextResource
    {
        public Guid VRLocalizationTextResourceId { get; set; }
        public string ResourceKey { get; set; }
        public Guid ModuleId { get; set; }
		public DateTime? CreatedTime { get; set; }
		public DateTime? LastModifiedTime { get; set; }
		public int? CreatedBy { get; set; }
		public int? LastModifiedBy { get; set; }
		public string DefaultValue { get; set; }
	}
}
