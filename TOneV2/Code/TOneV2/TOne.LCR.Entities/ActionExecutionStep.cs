using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public class ActionExecutionStep<T>
    {
        public T Action { get; set; }

        /// <summary>
        /// i.e. dont go to next step if action found and executed
        /// </summary>
        public bool IsEndAction { get; set; }

        public ActionExecutionStep<T> NextStep { get; set; }
    }
}
