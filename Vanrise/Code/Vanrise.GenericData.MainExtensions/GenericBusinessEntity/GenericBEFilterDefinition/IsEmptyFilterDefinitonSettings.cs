using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common.Business;


namespace Vanrise.GenericData.MainExtensions
{
    public class IsEmptyFilterDefinitonSettings : GenericBEFilterDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("177D3C5C-4A3D-4B3E-B63C-60B1021C0C40"); } }
        public override string RuntimeEditor { get { return "vr-genericdata-genericbe-filterruntime-isempty"; }  }
        public bool IsRequired { get; set; }
        public string FieldName { get; set; }
        public IsEmptyFilterDefinitonSettingsField AllField { get; set; }
        public IsEmptyFilterDefinitonSettingsField NullField { get; set; }
        public IsEmptyFilterDefinitonSettingsField NotNullField { get; set; }
		public override void ApplyTranslation(IGenericBETranslationContext context)
		{
			VRLocalizationManager vrLocalizationManager = new VRLocalizationManager();
			
			if (AllField != null)
			{
				if (!String.IsNullOrEmpty(AllField.Resourcekey))
				{
					AllField.Title = vrLocalizationManager.GetTranslatedTextResourceValue(AllField.Resourcekey, AllField.Title, context.LanguageId);
				}
			}
			if (NullField != null)
			{
				if (!String.IsNullOrEmpty(NullField.Resourcekey))
				{
					NullField.Title = vrLocalizationManager.GetTranslatedTextResourceValue(NullField.Resourcekey, NullField.Title, context.LanguageId);
				}
			}
			if (NotNullField != null)
			{
				if (!String.IsNullOrEmpty(NotNullField.Resourcekey))
				{
					NotNullField.Title = vrLocalizationManager.GetTranslatedTextResourceValue(NotNullField.Resourcekey, NotNullField.Title, context.LanguageId);
				}
			}

		}
	}
	
    public class IsEmptyFilterDefinitonSettingsField
    {
        public string Title { get; set; }
        public string Resourcekey { get; set; }
        public bool IsDefault { get; set; }
        public bool RemoveFromSelector { get; set; }
    }
}
