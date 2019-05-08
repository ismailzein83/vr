using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions
{
	public class TabsContainerEditorDefinitionSetting : VRGenericEditorDefinitionSetting
	{
		public override Guid ConfigId
		{
			get { return new Guid("AD2D93E0-0C06-4EBE-B7A9-BF380C256EEE"); }
		}
		public List<VRTabContainer> TabContainers { get; set; }
		public override string RuntimeEditor
		{
			get
			{
				return "vr-genericdata-tabscontainereditor-runtime";
			}
		}

		public override void TryTranslate()
		{
			VRLocalizationManager vrLocalizationManager = new VRLocalizationManager();
			if (TabContainers != null)
			{
				foreach (var tabContainer in TabContainers)
				{
					if (!String.IsNullOrEmpty(tabContainer.TextResourceKey))
					{
						tabContainer.TabTitle = vrLocalizationManager.GetTranslatedTextResourceValue(tabContainer.TextResourceKey, tabContainer.TabTitle);
					}
					tabContainer.TabSettings.TryTranslate();
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
                        columnsAttribute.AddRange(tabContainer.TabSettings.GetGridColumnsAttributes(context));
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
