using System;
using Vanrise.Common.Business;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions
{
    public class ChildEntityFilterDefinitionSettings : GenericBEFilterDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("F26CFFAE-1816-4CFC-9E62-6AD4330A1CBA"); } }

        public override string RuntimeEditor { get { return "vr-genericdata-genericbe-filterruntime-childentity"; } }

        public string FieldTitle { get; set; }

        public DataRecordFieldType FieldType { get; set; }

        public bool IsRequired { get; set; }
        
        public Guid ChildBusinessEntityDefinitionId { get; set; }

        public string ChildInputFieldName { get; set; }

        public dynamic ChildInputDefaultFieldValues { get; set; }

        public string ChildOutputFieldName { get; set; }

        public string ParentMappedFieldName { get; set; }

        public string TextResourceKey { get; set; }

        public override void ApplyTranslation(IGenericBETranslationContext context)
        {
            VRLocalizationManager vrLocalizationManager = new VRLocalizationManager();
            if (!String.IsNullOrEmpty(TextResourceKey))
            {
                FieldTitle = vrLocalizationManager.GetTranslatedTextResourceValue(TextResourceKey, FieldTitle, context.LanguageId);
            }
        }
    }
}