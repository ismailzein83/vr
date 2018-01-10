using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class DAProfCalcAnalysisPeriod
    {
        public int AnalysisPeriodTimeBack { get; set; }

        public DAProfCalcTimeUnit AnalysisPeriodTimeUnit { get; set; }

        public int GetPeriodInMinutes()
        {
            return this.AnalysisPeriodTimeBack * Vanrise.Common.Utilities.GetEnumAttribute<DAProfCalcTimeUnit, DAProfCalcTimeUnitAttribute>(this.AnalysisPeriodTimeUnit).Value;
        }
    }
}
