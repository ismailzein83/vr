using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
	public class VRLocalizationTextResourceTranslation
	{
		public Guid VRLocalizationTextResourceTranslationId { get; set; }
		public Guid ResourceId { get; set; }
		public Guid LanguageId { get; set; }
		public string Value { get; set; }
		public DateTime? CreatedTime { get; set; }
		public DateTime? LastModifiedTime { get; set; }
		public int? CreatedBy { get; set; }
		public int? LastModifiedBy { get; set; }
	}
	public class VRLocalizationTextResourceTranslationsById : Dictionary<Guid, VRLocalizationTextResourceTranslation>
	{

	}
}
