using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions
{
    public class TabsContainerEditorDefinitionSetting : VRGenericEditorDefinitionSetting
    {
        public override Guid ConfigId { get { return new Guid("AD2D93E0-0C06-4EBE-B7A9-BF380C256EEE"); } }

        public override string RuntimeEditor { get { return "vr-genericdata-tabscontainereditor-runtime"; } }

        public List<VRTabContainer> TabContainers { get; set; }

        public override void ApplyTranslation(IGenericBETranslationContext context)
        {
            VRLocalizationManager vrLocalizationManager = new VRLocalizationManager();
            if (TabContainers != null)
            {
                foreach (var tabContainer in TabContainers)
                {
                    if (!String.IsNullOrEmpty(tabContainer.TextResourceKey))
                    {
                        tabContainer.TabTitle = vrLocalizationManager.GetTranslatedTextResourceValue(tabContainer.TextResourceKey, tabContainer.TabTitle,context.LanguageId);
                    }
                    tabContainer.TabSettings.ApplyTranslation(context);
                }
            }
        }

        public override List<GridColumnAttribute> GetGridColumnsAttributes(IGetGenericEditorColumnsInfoContext context)
        {
            List<GridColumnAttribute> columnsAttribute = new List<GridColumnAttribute>();

            if (TabContainers != null && TabContainers.Count > 0)
            {
                foreach (var tabContainer in TabContainers)
                {
                    if (tabContainer.TabSettings != null)
                    {
                        var attributes = tabContainer.TabSettings.GetGridColumnsAttributes(new GetGenericEditorColumnsInfoContext
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

    public class VRTabContainer
    {
        public string TextResourceKey { get; set; }
        public string TabTitle { get; set; }
        public bool ShowTab { get; set; }
        public VRGenericEditorDefinitionSetting TabSettings { get; set; }
    }

}