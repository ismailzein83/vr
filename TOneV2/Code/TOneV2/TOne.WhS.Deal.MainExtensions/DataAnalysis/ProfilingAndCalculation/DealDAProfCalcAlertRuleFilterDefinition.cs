using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace TOne.WhS.Deal.MainExtensions.DataAnalysis.ProfilingAndCalculation
{
    public class DealDAProfCalcAlertRuleFilterDefinition : DAProfCalcAlertRuleFilterDefinition
    {
        public override Guid ConfigId { get { return new Guid("9CA79A5E-D8F5-4927-96D3-0F7568F87265"); } }

        public override string RuntimeEditor { get { return ""; } }
    }
}