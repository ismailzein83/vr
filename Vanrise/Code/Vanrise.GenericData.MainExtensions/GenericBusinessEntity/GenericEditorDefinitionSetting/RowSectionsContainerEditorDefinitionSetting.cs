using System;
using System.Collections.Generic;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions
{
    public class RowSectionsContainerEditorDefinitionSetting : VRGenericEditorDefinitionSetting
    {
        public override Guid ConfigId { get { return new Guid("E7AB5AD4-2BBC-4120-9278-2261DEDAB455"); } }

        public override string RuntimeEditor { get { return "vr-genericdata-rowsectionscontainereditor-runtime"; } }

        public List<VRSectionContainer> RowSectionContainers { get; set; }

        public override void ApplyTranslation(IGenericEditorTranslationContext context)
        {
            VRLocalizationManager vrLocalizationManager = new VRLocalizationManager();
            if (RowSectionContainers != null)
            {
                foreach (var sectionContainer in RowSectionContainers)
                {
                    if (!String.IsNullOrEmpty(sectionContainer.TextResourceKey))
                    {
                        sectionContainer.SectionTitle = vrLocalizationManager.GetTranslatedTextResourceValue(sectionContainer.TextResourceKey, sectionContainer.SectionTitle,context.LanguageId);
                    }
                    sectionContainer.SectionSettings.ApplyTranslation(context);
                }
            }
        }

        public override Dictionary<string,GridColumnAttribute> GetGridColumnsAttributes(IGetGenericEditorColumnsInfoContext context)
        {
            Dictionary<string, GridColumnAttribute> columnsAttribute = new Dictionary<string, GridColumnAttribute>();

            if (RowSectionContainers != null && RowSectionContainers.Count > 0)
            {
                foreach (var sectionContainer in RowSectionContainers)
                {
                    if (sectionContainer.SectionSettings != null)
                    {
                        var attributes = sectionContainer.SectionSettings.GetGridColumnsAttributes(new GetGenericEditorColumnsInfoContext
                        {
                            DataRecordTypeId = context.DataRecordTypeId
                        });
                        if (attributes != null)
                        {
                            foreach (var attribute in attributes)
                            {
                                if (!columnsAttribute.ContainsKey(attribute.Key))
                                    columnsAttribute.Add(attribute.Key, attribute.Value);
                            }
                        }
                    }
                }
            }
            return columnsAttribute;
        }
    }
}