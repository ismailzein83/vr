using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    }
    public class VRTabContainer
    {
        public string TabTitle { get; set; }
        public bool ShowTab { get; set; }
        public VRGenericEditorDefinitionSetting TabSettings { get; set; }
    }
}
