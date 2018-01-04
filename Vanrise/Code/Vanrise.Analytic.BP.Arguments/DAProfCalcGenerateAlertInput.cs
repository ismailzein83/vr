using System;
using System.Collections.Generic;
using Vanrise.Analytic.Entities;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.Analytic.BP.Arguments
{
    public enum DAProfCalcTimeUnit
    {
        [DAProfCalcTimeUnitAttribute(1440)]
        Days = 0,
        [DAProfCalcTimeUnitAttribute(60)]
        Hours = 1,
        [DAProfCalcTimeUnitAttribute(1)]
        Minutes = 2
    }

    public class DAProfCalcTimeUnitAttribute : Attribute
    {
        public DAProfCalcTimeUnitAttribute(int value)
        {
            this.Value = value;
        }
        public int Value { get; set; }
    }

    public class DAProfCalcGenerateAlertInput : BaseProcessInputArgument
    {
        public Guid AlertRuleTypeId { get; set; }

        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public DAProfCalcChunkTimeEnum? ChunkTime { get; set; }

        public override string GetTitle()
        {
            return String.Format("Data Analysis Profiling And Calculation Generate Alert Process from {0} to {1}", FromTime.ToString("yyyy-MM-dd HH:mm:ss"), ToTime.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        public override void MapExpressionValues(Dictionary<string, object> evaluatedExpressions)
        {
            if (evaluatedExpressions.ContainsKey("ScheduleTime"))
            {
                var effectiveDate = (DateTime)evaluatedExpressions["ScheduleTime"];
                int timeBack = int.Parse(evaluatedExpressions["TimeBack"].ToString());
                int period = int.Parse(evaluatedExpressions["Period"].ToString());

                DAProfCalcTimeUnit timeBackTimeUnit = (DAProfCalcTimeUnit)int.Parse(evaluatedExpressions["TimeBackTimeUnit"].ToString());
                DAProfCalcTimeUnit periodTimeUnit = (DAProfCalcTimeUnit)int.Parse(evaluatedExpressions["PeriodTimeUnit"].ToString());

                DAProfCalcTimeUnitAttribute timeBackAttribute = Vanrise.Common.Utilities.GetEnumAttribute<DAProfCalcTimeUnit,DAProfCalcTimeUnitAttribute>(timeBackTimeUnit);
                DAProfCalcTimeUnitAttribute periodAttribute = Vanrise.Common.Utilities.GetEnumAttribute<DAProfCalcTimeUnit,DAProfCalcTimeUnitAttribute>(periodTimeUnit);

                this.FromTime = effectiveDate.AddMinutes(-1 * timeBack * timeBackAttribute.Value);
                this.ToTime = this.FromTime.AddMinutes(period * periodAttribute.Value);
            }
        }
    }
}