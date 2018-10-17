using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public abstract class DAProfCalcAlertRuleFilterDefinition
    {
        public abstract Guid ConfigId { get; }

        public abstract string RuntimeEditor { get; }
    }
}
