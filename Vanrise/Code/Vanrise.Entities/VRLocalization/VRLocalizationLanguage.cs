using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRLocalizationLanguage
    {
        public Guid VRLanguageId { get; set; }

        public string Name { get; set; }

        public Guid? ParentLanguageId { get; set; }

        public VRLocalizationLanguageSettings Settings { get; set; }
		public DateTime? CreatedTime { get; set; }
		public DateTime? LastModifiedTime { get; set; }
		public int? CreatedBy { get; set; }
		public int? LastModifiedBy { get; set; }

	}

    public class VRLocalizationLanguageSettings
    {
        public bool IsRTL { get; set; }
    }
}
