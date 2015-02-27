using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public class RuleActionExecutionStep
    {
        public BaseRouteAction Action { get; set; }

        /// <summary>
        /// i.e. dont go to next step if action found and executed
        /// </summary>
        public bool IsEndAction { get; set; }

        public RuleActionExecutionStep NextStep { get; set; }
    }
}
