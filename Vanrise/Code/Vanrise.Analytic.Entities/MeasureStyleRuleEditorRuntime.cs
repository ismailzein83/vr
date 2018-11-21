using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class MeasureStyleRuleEditorRuntime
    {
        public List<MeasureStyleRuleRuntime> MeasureStyleRulesRuntime { get; set; }
    }
    public enum StyleCodeEnum // to be removed when the reference will be added
    {
        [Description("Red")]
        Red = 0,

        [Description("Green")]
        Green = 1,

        [Description("Yellow")]
        Yellow = 2,

        [Description("Blue")]
        Blue = 3,

        [Description("Red Text Value")]
        RedTextValue = 4,

        [Description("Green Text Value")]
        GreenTextValue = 5
    }
}
