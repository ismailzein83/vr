using System;
using System.Collections.Generic;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions
{
    public class SectionsContainerEditorDefinitionSetting : VRGenericEditorDefinitionSetting
    {
        public override Guid ConfigId { get { return new Guid("51FA6D36-7D51-44B5-8618-E78FA1E33761"); } }

        public override string RuntimeEditor { get { return "vr-genericdata-sectionscontainereditor-runtime"; } }

        public List<VRSectionContainer> SectionContainers { get; set; }

        public override void ApplyTranslation(IGenericBETranslationContext context)
        {
            VRLocalizationManager vrLocalizationManager = new VRLocalizationManager();
            if (SectionContainers != null)
            {
                foreach (var sectionContainer in SectionContainers)
                {
                    if (!String.IsNullOrEmpty(sectionContainer.TextResourceKey))
                    {
                        sectionContainer.SectionTitle = vrLocalizationManager.GetTranslatedTextResourceValue(sectionContainer.TextResourceKey, sectionContainer.SectionTitle,context.LanguageId);
                    }
                    sectionContainer.SectionSettings.ApplyTranslation(context);
                }
            }
        }

        public override List<GridColumnAttribute> GetGridColumnsAttributes(IGetGenericEditorColumnsInfoContext context)
        {
            List<GridColumnAttribute> columnsAttribute = new List<GridColumnAttribute>();

            if (SectionContainers != null && SectionContainers.Count > 0)
            {
                foreach (var sectionContainer in SectionContainers)
                {
                    if (sectionContainer.SectionSettings != null)
                    {
                        var attributes = sectionContainer.SectionSettings.GetGridColumnsAttributes(new GetGenericEditorColumnsInfoContext
                        {
                            DataRecordTypeId = context.DataRecordTypeId
                        });

                        if (attributes != null && attributes.Count > 0)
                            columnsAttribute.AddRange(attributes);
                    }
                }
            }
            return columnsAttribute;
        }
    }

    public class VRSectionContainer
    {
        public string TextResourceKey { get; set; }
        public string SectionTitle { get; set; }
        public int ColNum { get; set; }
        public VRGenericEditorDefinitionSetting SectionSettings { get; set; }
    }

}