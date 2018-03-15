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

        public string GetDescription()
        {
            DAProfCalcTimeUnitAttribute daProfCalcTimeUnitAttribute = Vanrise.Common.Utilities.GetEnumAttribute<DAProfCalcTimeUnit, DAProfCalcTimeUnitAttribute>(this.AnalysisPeriodTimeUnit);
            return string.Format("{0} {1}", this.AnalysisPeriodTimeBack, this.AnalysisPeriodTimeBack > 1 ? daProfCalcTimeUnitAttribute.PluralDescription : daProfCalcTimeUnitAttribute.SingularDescription);
        }
    }
}
