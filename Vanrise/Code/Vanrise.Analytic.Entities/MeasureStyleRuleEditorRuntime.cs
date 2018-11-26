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
        [Description("Excellent")]
        Excellent = 0,

        [Description("Good")]
        Good = 1,

        [Description("Fair")]
        Fair = 2,

        [Description("Poor")]
        Poor = 3,

        [Description("Bad")]
        Bad = 4,
    }
}
