using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions
{
    public class ChildEntityFilterDefinitionSettings : GenericBEFilterDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("F26CFFAE-1816-4CFC-9E62-6AD4330A1CBA"); } }

        public override string RuntimeEditor { get { return "vr-genericdata-genericbe-filterruntime-childentity"; } }

        public DataRecordFieldType FieldType { get; set; }

        public Guid ChildBusinessEntityDefinitionId { get; set; }

        public string SearchChildFieldName { get; set; }

        public bool IsRequired { get; set; }

        public string SearchChildFieldTitle { get; set; }

        public string MappedFieldName { get; set; }

        public string MappedChildFieldName { get; set; }

        public string TextResourceKey { get; set; }

        public override void ApplyTranslation(IGenericBETranslationContext context)
        {
            VRLocalizationManager vrLocalizationManager = new VRLocalizationManager();
            if (!String.IsNullOrEmpty(TextResourceKey))
            {
                SearchChildFieldTitle = vrLocalizationManager.GetTranslatedTextResourceValue(TextResourceKey, SearchChildFieldTitle, context.LanguageId);
            }
        }
    }
}
