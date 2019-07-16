using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public abstract class DAProfCalcAnalysisPeriodSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract int GetPeriodInMinutes(IDAProfCalcAnalysisPeriodContext context);
    }

    public interface IDAProfCalcAnalysisPeriodContext
    {
        DateTime EffectiveDate { get; }
    }

    public class DAProfCalcAnalysisPeriodContext : IDAProfCalcAnalysisPeriodContext
    {
        public DateTime EffectiveDate { get; set; }
    }
}