using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class GenericEditorRuntimeField:GenericUIRuntimeField
    {
        public bool IsRequired { get; set; }
        public bool IsDisabled { get; set; }
        public bool ShowAsLabel { get; set; }
        public int? FieldWidth { get; set; }
        public bool HideLabel { get; set; }
        public bool ReadOnly { get; set; }
    }
}
